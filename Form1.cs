using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace ComercialSF_APILinker
{
    public partial class Form1 : Form
    {
        bool _running = false;
        int _bar = 0;
        string _title_confirm = "Confirmar acción";
        string _license_file = "";
        int _cfg_robot = 0;
        string _path_logs = "";
        string _api_server = "";
        List<cStatus> controlStatus = new List<cStatus>();

        DateTime lastRead = DateTime.MinValue;
        cIniFile _INI = new cIniFile();
        cLog _LOG = new cLog();
        NeoCaller _CALLER = new NeoCaller();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Reset();
            this.ToggleTimer(true);
            tmrTray.Enabled = true;
        }
        private void btnRun_Click(object sender, EventArgs e)
        {
            this.ToggleTimer(true);
        }
        private void tmrRun_Tick(object sender, EventArgs e)
        {
            this.RunTick();
        }
        private void tmrBar_Tick(object sender, EventArgs e)
        {
            this._bar += 1;
            if (this._bar > 100) { this._bar = 0; }
            this.pBar.Value = this._bar;
            Application.DoEvents();
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Show();
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                Hide();
                this.WindowState = FormWindowState.Minimized;
            }
        }
        private void tmrTray_Tick(object sender, EventArgs e)
        {
            tmrTray.Enabled = false;
            Hide();
            notifyIcon1.Visible = true;
        }

        private void Reset()
        {
            /*Setting for run*/
            this.LoadConfigINI();
            this.tmrRun.Enabled = false;
            this.tmrBar.Enabled = false;
            this.lvSource.Enabled = true;
            this._bar = 0;
            this.pBar.Value = this._bar;
            this.btnRun.Text = "Activar proceso";
            //_ = this.FilesPopulate(true);
        }
        private void LoadConfigINI()
        {
            this._license_file = (this._INI.Directory + "\\license.lic");
            int.TryParse(this._INI.Read("cfgRobot"), out this._cfg_robot);
            this._path_logs = this._INI.Read("pathLogs");
            this._api_server = this._INI.Read("apiServer");
            if (this._path_logs == "") { this._INI.Write("pathLogs", this._INI.Directory + "\\logs"); }
            if (this._api_server == "") { this._INI.Write("apiServer", "http://localhost:41000/"); }
        }
        private bool ValidateConfig()
        {
            this.LoadConfigINI();
            bool _ret = true;
            string _message = "";
            if (this._cfg_robot == 0)
            {
                _message += "· No se ha definido valor para cfgRobot en archivo INI " + "\n";
            }
            if (this._api_server == "")
            {
                _message += "· No se ha definido valor para apiServer en archivo INI " + "\n";
            }
            if (!Directory.Exists(this._path_logs)) { _message += "· No existe el directorio " + this._path_logs + "\n"; }
            _LOG.toDisk = true;
            if (_message != "")
            {
                _ret = false;
                _message += "NO SE PROCESARÁN LOS ARCHIVOS EN LOS DIRECTORIOS DE TRABAJO\n";
                _message += "SISTEMA DETENIDO\n";
                if (this.tmrRun.Enabled) { this.ToggleTimer(false); }
            }
            else {
                _message += "CONFIGURACIÓN VERIFICADA - 100% OPERATIVO\n";
            }
            this.lblMessage.Text = _message;
            _CALLER._SERVER = this._api_server;
            return _ret;
        }
        private void updateINI()
        {
            this._INI.Write("pathErrors", this.textBox3.Text);
        }
        private void ToggleTimer(bool _confirm) {
            this._running = !this._running;
            if (this._running)
            {
                this.Reset();
                this.tmrRun.Enabled = true;
                this.tmrBar.Enabled = true;
                this.lvSource.Enabled = false;
                this.btnRun.Text = "Detener proceso";
            }
            else
            {
                DialogResult result = DialogResult.Yes;
                if (_confirm)
                {
                    string _message = "El proceso de notificaciones vía API será detenido  ¿Confirma la acción?";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    result = MessageBox.Show(_message, this._title_confirm, buttons);
                }
                if (result == DialogResult.Yes) { this.Reset(); }
            }
        }
        private bool IsBetween(TimeSpan start, TimeSpan end)
        {
            var time = DateTime.Now.TimeOfDay;
            if (start <= end) { return time >= start && time <= end; }
            return time >= start || time <= end;
        }
        private String[] getFilesFrom(String searchFolder, String[] filters, bool isRecursive)
        {
            List<String> filesFound = new List<String>();
            var searchOption = isRecursive ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, String.Format("*.{0}", filter), searchOption));
            }
            return filesFound.ToArray();
        }

        private async Task<bool> RunTick()
        {
            this.tmrRun.Enabled = false;
            if (this.ValidateConfig()) { await this.processFilesAsync(); }
            await this.FilesPopulate(false);
            this.tmrRun.Enabled = true;
            return true;
        }
        private async Task FilesPopulate(bool _force)
        {
            if (this.ValidateConfig())
            {
                cRespuesta _profiles = await _CALLER.getProfiles(this._cfg_robot);
                if (_profiles.status == "OK")
                {
                    this.lvSource.Clear();
                    this.lvSource.BeginUpdate();
                    this.lvSource.Columns.Add("Archivo");
                    this.lvSource.Columns.Add("Source");
                    this.lvSource.Columns.Add("Target");
                    this.lvSource.Columns.Add("Storage");
                    this.lvSource.Columns.Add("Tipo");
                    this.lvSource.Columns.Add("Mime");
                    this.lvSource.Columns.Add("U");
                    this.lvSource.Columns.Add("C");
                    this.lvSource.Columns.Add("dbSource");
                    this.lvSource.Columns.Add("dbTarget");
                    this.lvSource.Columns.Add("F");
                    this.lvSource.Columns.Add("P");
                    this.lvSource.Columns.Add("S");

                    this.lvSource.Sorting = System.Windows.Forms.SortOrder.Ascending;

                    foreach (cProfile _profile in _profiles.data)
                    {
                        try
                        {
                            this._LOG.LogWriter("PROCESANDO CLIENTE: INICIO-> " + DateTime.Now.ToString() + " ID-> " + _profile.id_client.ToString(), this._path_logs);
                            if (!_CALLER.asyncStatus.Any(o => (o.id_profile == _profile.id)))
                            {
                                cAsync _async = new cAsync();
                                _async.id_client = _profile.id_client;
                                _async.id_profile = _profile.id;
                                _async.ExecuteModPre = false;
                                _async.ExecuteModStatus = false;
                                _async.ExecuteModPost = false;
                                _CALLER.asyncStatus.Add(_async);
                            }

                            var result = _CALLER.asyncStatus.Select(i =>
                            {
                                if (i.id_profile == _profile.id) {
                                    if (!i.ExecuteModPre)
                                    {
                                        try {
                                            i.ExecuteModPre = true;
                                            Task<cRespuesta> _tModPre = this.ExecuteModPre(_profile); 
                                        } catch (Exception ex) { }
                                    }
                                    if (!i.ExecuteModStatus && _profile.mod_status == 1)
                                    {
                                        try {
                                            i.ExecuteModStatus = true;
                                            Task<cRespuesta> _tModStatus = this.ExecuteModStatus(_profile);
                                        } catch (Exception ex) { }
                                    }
                                    if (!i.ExecuteModPost && _profile.mod_post == 1)
                                    {
                                        /*Módulos de post procesamiento!*/
                                        try {
                                            i.ExecuteModPost = true;
                                            Task<cRespuesta> _tModPost = this.ExecuteModPost(_profile); 
                                        } catch (Exception ex) { }
                                    }
                                }
                                return i;
                            }).ToList();

                            /*Módulo de conteo normal*/
                            if (!this.IsBetween(TimeSpan.Parse(_profile.time_from.ToString("HH:mm")), TimeSpan.Parse(_profile.time_to.ToString("HH:mm")))) { break; }
                            if (Directory.Exists(_profile.unc_source))
                            {
                                try { Directory.CreateDirectory(_profile.unc_storage); } catch (Exception rex) { }
                                try {
                                    /*
									switch (_profile.id_client)
									{
										case 16:
											_profile.unc_target = _profile.unc_target + "\\Sede";
											break;
                                        default:
											Directory.CreateDirectory(_profile.unc_target);
											break;
									}
                                    */
									Directory.CreateDirectory(_profile.unc_target);
								}
								catch (Exception rex) { }

								string[] folders = Directory.GetDirectories(_profile.unc_source, "*", System.IO.SearchOption.AllDirectories);
                                foreach (string folder in folders)
                                {
                                    string _new = folder.Replace(_profile.unc_source, _profile.unc_target);
                                    if (!String.IsNullOrEmpty(_profile.sufix_subdirs)) { _new += (" " + DateTime.Now.ToString(_profile.sufix_subdirs.ToString())); }
                                    try {
                                        /*
										switch (_profile.id_client)
										{
											case 16:
												_new = _new.Replace("Sede\\", "Sede");
												break;
											default:
												break;
										}
                                        */
										Directory.CreateDirectory(_new); 
                                    } catch (Exception rex) { }
                                }
                                String[] _filters = new String[] { };
                                if (_profile.pdf == 1)
                                {
                                    Array.Resize<string>(ref _filters, _filters.Length + 1);
                                    _filters[_filters.Length - 1] = "pdf";
                                }
                                if (_profile.tiff == 1)
                                {
                                    Array.Resize<string>(ref _filters, _filters.Length + 1);
                                    _filters[_filters.Length - 1] = "tif";
                                    Array.Resize<string>(ref _filters, _filters.Length + 1);
                                    _filters[_filters.Length - 1] = "tiff";
                                }
								if (_profile.jpeg == 1)
								{
									Array.Resize<string>(ref _filters, _filters.Length + 1);
									_filters[_filters.Length - 1] = "jpg";
									Array.Resize<string>(ref _filters, _filters.Length + 1);
									_filters[_filters.Length - 1] = "jpeg";
								}

								int _i = 0;
                                int _limit = 50;
                                string[] files = this.getFilesFrom(_profile.unc_source, _filters, true);
                                foreach (string file in files)
                                {
                                    if (_i >= _limit) { break; }
                                    FileInfo fi = new FileInfo(file);
                                    if (fi.CreationTime <= DateTime.Now.AddMinutes(-_profile.mm_alive))
                                    {
                                        string fileName = Path.GetFileName(file);
                                        string _dir = Path.GetDirectoryName(file);
                                        string _type = Path.GetExtension(file);
                                        string _mime = "";
                                        switch (_type)
                                        {
                                            case ".pdf":
                                                _mime = "application/pdf";
                                                break;
                                            case ".tif":
                                            case ".tiff":
                                                _mime = "image/tiff";
                                                break;
											case ".jpg":
											case ".jpeg":
												_mime = "image/jpeg";
												break;
                                        }

										string _target = _dir.Replace(_profile.unc_source, _profile.unc_target);
                                        /*
										switch (_profile.id_client)
										{
											case 16:
                                                _target = _target.Replace("Sede\\", "Sede");
												break;
										}
                                        */

										if (!String.IsNullOrEmpty(_profile.sufix_subdirs)) { _target += (" " + DateTime.Now.ToString(_profile.sufix_subdirs.ToString())); }
                                        ListViewItem item = new ListViewItem(
                                            new string[] {
                                            fileName,
                                            _dir,
                                            _target,
                                            _profile.unc_storage,
                                            _type,
                                            _mime,
                                            _profile.id_user.ToString(),
                                            _profile.id_client.ToString(),
                                            _profile.db_source,
                                            _profile.db_target,
                                            _profile.mod_files.ToString(),
                                            _profile.mod_pages.ToString(),
                                            _profile.mod_status.ToString()
                                            });
                                        if (!this.lvSource.Items.Contains(item)) { this.lvSource.Items.Add(item); }
                                        _i += 1;
                                    }
                                }
                                Dictionary<int, int> columnSize = new Dictionary<int, int>();
                                this.lvSource.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                                foreach (ColumnHeader colHeader in this.lvSource.Columns) { columnSize.Add(colHeader.Index, colHeader.Width); }
                                this.lvSource.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                                foreach (ColumnHeader colHeader in this.lvSource.Columns)
                                {
                                    int nColWidth;
                                    if (columnSize.TryGetValue(colHeader.Index, out nColWidth)) { colHeader.Width = Math.Max(nColWidth, colHeader.Width); } else { colHeader.Width = Math.Max(50, colHeader.Width); }
                                }
                                this.lvSource.EndUpdate();
                            }
                        }
                        catch (Exception e)
                        {
                            this._LOG.LogWriter("ERROR - DETALLE: " + e.ToString(), this._path_logs);
                        }
                    }
                }
            }
        }
        private async Task processFilesAsync()
        {
            foreach (ListViewItem item in lvSource.Items)
            {
                string fileSource = (item.SubItems[1].Text + "\\" + item.SubItems[0].Text);
                string fileTarget = (item.SubItems[2].Text + "\\" + item.SubItems[0].Text);
                string fileStorage = (item.SubItems[3].Text + "\\" + item.SubItems[0].Text);
                try
                {
                    int _pages = 1;
                    switch (item.SubItems[4].Text)
                    {
						case ".jpeg":
						case ".jpg":
							_pages = 1;
							break;
						case ".tiff":
                        case ".tif":
                            Image img = Image.FromFile(fileSource);
                            Guid ID = img.FrameDimensionsList[0];
                            FrameDimension fd = new FrameDimension(ID);
                            _pages = img.GetFrameCount(fd);
                            img.Dispose();
                            break;
                        case ".pdf":
                            using (StreamReader sr = new StreamReader(File.OpenRead(fileSource)))
                            {
                                Regex regex = new Regex(@"/Type\s*/Page[^s]");
                                MatchCollection matches = regex.Matches(sr.ReadToEnd());
                                _pages = matches.Count;
                            }
                            break;
                    }

                    File.Copy(fileSource, fileTarget, true);
                    File.Copy(fileSource, fileStorage, true);
                    cCounters _fields = new cCounters();
                    _fields.id_user = int.Parse(item.SubItems[6].Text);
                    _fields.id_client = int.Parse(item.SubItems[7].Text);
                    _fields.filename = item.SubItems[0].Text;
                    _fields.filepath = item.SubItems[1].Text;
                    _fields.pages = _pages;
                    _fields.extension = item.SubItems[4].Text;
                    _fields.mime_type = item.SubItems[5].Text;
                    _fields.json = "";
                    cRespuesta _respuesta = await _CALLER.sendDataViaAPI(_fields, this._path_logs);
                    if (_respuesta.status == "OK") { File.Delete(fileSource); }
                }
                catch (Exception e)
                {
                    this._LOG.LogWriter("ERROR: No se ha podido procesar el archivo " + fileSource + ",DETALLE: " + e.ToString(), this._path_logs);
                }
                item.Remove();
                Application.DoEvents();
            }
        }

        private async Task<cRespuesta> ExecuteModStatus(cProfile _profile)
        {
            cRespuesta _respuesta = new cRespuesta();
            _respuesta.status = "OK";
            _respuesta.message = "";
            SqlConnection _sqlConnectionSource = null;
            if (_profile.db_source != "" && _profile.sql_status != "")
            {
                try
                {
                    /*Purge old controled Status List*/
                    foreach (cStatus _status in controlStatus.ToList())
                    {
                        TimeSpan span = DateTime.Now.Subtract(_status.created);
                        if (span.Days >= 1) { controlStatus.Remove(_status); }
                    }
                    _sqlConnectionSource = new SqlConnection(_profile.db_source);
                    _sqlConnectionSource.Open();
                    SqlCommand _sqlCommand = new SqlCommand("", _sqlConnectionSource);
                    _sqlCommand.CommandText = _profile.sql_status;
                    _sqlCommand.Parameters.AddWithValue("@ID", String.Format("{0}", _profile.id_client));
                    DataTable dtTable = new DataTable("dtStatus");
                    dtTable.Load(_sqlCommand.ExecuteReader());
                    foreach (DataRow _row in dtTable.Rows)
                    {
                        cStatus _status = new cStatus();
                        _status.id_file = _row["id"].ToString();
                        _status.id_client = _profile.id_client;
                        _status.filename = _row["filename"].ToString();
                        _status.status = _row["status"].ToString();
                        _status.created = DateTime.Now;
                        if (!controlStatus.Any(o => (o.id_file == _status.id_file && o.filename == _status.filename && o.status == _status.status)))
                        {
                            controlStatus.Remove(_status);
                            controlStatus.Add(_status);
                            _respuesta = await _CALLER.sendStatusViaAPI(_status, this._path_logs);
                        }
                    }
                    _sqlConnectionSource.Close();
                    _sqlConnectionSource.Dispose();
                }
                catch (Exception ex)
                {
                    _respuesta = new cRespuesta();
                    _respuesta.status = "ERROR";
                    _respuesta.message = ex.ToString();
                    _sqlConnectionSource = null;
                }
            }
            var result = _CALLER.asyncStatus.Select(i =>
            {
                if (i.id_profile == _profile.id) { i.ExecuteModStatus = false; }
                return i;
            }).ToList();
            return _respuesta;
        }
        private async Task<cRespuesta> ExecuteModPre(cProfile _profile)
        {
            cRespuesta _respuesta = new cRespuesta();
            try
            {
                _respuesta = await _CALLER.sendPreViaAPI(_profile, "", this._path_logs);
            }
            catch (Exception e)
            {
                _respuesta = new cRespuesta();
                _respuesta.status = "ERROR";
                _respuesta.message = e.ToString();
                this._LOG.LogWriter("ERROR - DETALLE: " + e.ToString(), this._path_logs);
            }
            var result = _CALLER.asyncStatus.Select(i =>
            {
                if (i.id_profile == _profile.id) { i.ExecuteModPre = false; }
                return i;
            }).ToList();
            return _respuesta;
        }
        private async Task<cRespuesta> ExecuteModPost(cProfile _profile)
        {
            cRespuesta _respuesta = new cRespuesta();
            _respuesta.status = "OK";
            _respuesta.message = "";
            if (Directory.Exists(_profile.post_unc_source))
            {
                string[] files = Directory.GetFiles(_profile.post_unc_source);
                foreach (string file in files)
                {
                    try
                    {
                        _respuesta = await _CALLER.sendPostViaAPI(_profile, file, this._path_logs);
                    }
                    catch (Exception e)
                    {
                        _respuesta = new cRespuesta();
                        _respuesta.status = "ERROR";
                        _respuesta.message = e.ToString();
                        this._LOG.LogWriter("ERROR - DETALLE: " + e.ToString(), this._path_logs);
                    }
                }
            }
            var result = _CALLER.asyncStatus.Select(i =>
            {
                if (i.id_profile == _profile.id) { i.ExecuteModPost = false; }
                return i;
            }).ToList();
            return _respuesta;
        }
    }
}

