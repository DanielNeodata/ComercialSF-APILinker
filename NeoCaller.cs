using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Encoder = System.Drawing.Imaging.Encoder;

namespace ComercialSF_APILinker
{
    public class NeoCaller
    {
        private cLog _LOG = new cLog();
        public List<cAsync> asyncStatus = new List<cAsync>();
        public string _SERVER = "http://localhost:41000/";
        //public string _SERVER = "https://clientes.cloud-capture.cl/";

        public async Task<cRespuesta> getProfiles(int id_user) {
            cRespuesta _respuesta = new cRespuesta();
            try
            {
                var clientR = new RestClient((this._SERVER + "api.backend/getProfiles"));
                var requestR = new RestRequest();
                requestR.Method = Method.Post;
                requestR.AddHeader("Content-Type", "application/json");
                requestR.AddParameter("id_user", id_user.ToString(), ParameterType.GetOrPost);
                var responseR =  await clientR.ExecutePostAsync(requestR);
                JsonSerializerSettings jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                _respuesta = JsonConvert.DeserializeObject<cRespuesta>(responseR.Content, jsonSetting);
                if (_respuesta.status == "ERROR") { throw new Exception(_respuesta.message); }
            }
            catch (Exception e)
            {
                _respuesta = new cRespuesta();
                _respuesta.status = "ERROR";
                _respuesta.message = e.ToString();
                this._LOG.LogWriter("ERROR: Previo a llamada API" + (this._SERVER + "api.backend/getProfiles") + ", DETALLE: " + e.ToString(), "");
            }
            return _respuesta;
        }
        public async Task<cRespuesta> sendPreViaAPI(cProfile _profile, string file, string sDirLog)
        {
            string _lastString = "";
            cRespuesta _respuesta = new cRespuesta();
            bool _withError = false;
            try
            {
                cPost _post = new cPost();
                _post.base64String = "";
                _post.fullpath = "";
                _post.filename = "";
                _post.id_client = _profile.id_client;
                _post.id_profile = _profile.id;
                _post.created = DateTime.Now;

                var clientR = new RestClient((this._SERVER + "api.backend/setPre"));
                var requestR = new RestRequest();
                requestR.RequestFormat = DataFormat.Json;
                requestR.Method = Method.Post;
                requestR.AddHeader("Content-Type", "application/json");
                requestR.AddJsonBody(JsonConvert.SerializeObject(_post));

                var responseR = await clientR.ExecutePostAsync(requestR);
                _lastString = responseR.Content.ToString();

                //this._LOG.LogWriter("URL: " + (this._SERVER + "api.backend/setPre"), sDirLog);
                //this._LOG.LogWriter("MSJ post data: " + JsonConvert.SerializeObject(_post), sDirLog);
                //this._LOG.LogWriter("RESPONSE: " + responseR.ToString(), sDirLog);

                JsonSerializerSettings jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                _respuesta = JsonConvert.DeserializeObject<cRespuesta>(responseR.Content, jsonSetting);

                if (_respuesta == null)
                {
                    _withError = true;
                    _respuesta = new cRespuesta();
                    _respuesta.code = "9999";
                    _respuesta.message = "Imposible realizar la operación solicitada";
                }
                else
                {
                    if (_respuesta.status == "ERROR") { _withError = (_respuesta.code.ToString() != "6003"); }
                }

                if (_withError) { throw new Exception("1 - " + _respuesta.message); }

                /* 
                 * Aca vuelve un array de files en el server que deben ser puesto 
                 * en el directory de entrada del cliente, según el profile!
                 */
                if (_respuesta.items != null)
                {
                    foreach (string _item in _respuesta.items)
                    {
                        _post = new cPost();
                        _post.base64String = "";
                        _post.fullpath = _item;
                        _post.filename = Path.GetFileName(_item);
                        _post.id_client = _profile.id_client;
                        _post.id_profile = _profile.id;
                        _post.created = DateTime.Now;

                        clientR = new RestClient((this._SERVER + "api.backend/getFilePre"));
                        requestR = new RestRequest();
                        requestR.RequestFormat = DataFormat.Json;
                        requestR.Method = Method.Post;
                        requestR.AddHeader("Content-Type", "application/json");
                        requestR.AddJsonBody(JsonConvert.SerializeObject(_post));
                        responseR = await clientR.ExecutePostAsync(requestR);

                        jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                        _respuesta = JsonConvert.DeserializeObject<cRespuesta>(responseR.Content, jsonSetting);
                        if (_respuesta.status == "OK")
                        {
                            /*poner el archivo recibido en el directorio _profile.unc_source*/
                            string _subdir = Path.GetFileName(Path.GetDirectoryName(_item));
                            string _dir = (_profile.unc_source + "\\");
                            if (_subdir != _profile.id_client.ToString()) { _dir += (_subdir + "\\"); }

                            if (!Directory.Exists(_dir)) { try { Directory.CreateDirectory(_dir); } catch (Exception rex) { } }
                            string _file = (_dir + Path.GetFileName(_item));
                            File.WriteAllBytes(_file, Convert.FromBase64String(_respuesta.message));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _respuesta = new cRespuesta();
                _respuesta.status = "ERROR";
                _respuesta.message = e.ToString();
                this._LOG.LogWriter("ERROR: Previo a llamada API" + (this._SERVER + "api.backend/setPre") + ", DETALLE: " + e.ToString(), sDirLog);
                this._LOG.LogWriter("ERROR: DETALLE RESPONSE -> " + _lastString, sDirLog);
            }
            return _respuesta;
        }
        public async Task<cRespuesta> sendDataViaAPI(cCounters _fields, string sDirLog) {
            cRespuesta _respuesta = new cRespuesta();
            try
            {
                var clientR = new RestClient((this._SERVER + "api.backend/setCounter"));
                var requestR = new RestRequest();
                requestR.Method = Method.Post;
                requestR.AddHeader("Content-Type", "application/json");
                requestR.AddParameter("id_user", _fields.id_user.ToString(), ParameterType.GetOrPost);
                requestR.AddParameter("id_client", _fields.id_client.ToString(), ParameterType.GetOrPost);
                requestR.AddParameter("filename", _fields.filename.ToString(), ParameterType.GetOrPost);
                requestR.AddParameter("filepath", _fields.filepath.ToString(), ParameterType.GetOrPost);
                requestR.AddParameter("pages", _fields.pages.ToString(), ParameterType.GetOrPost);
                requestR.AddParameter("extension", _fields.extension.ToString(), ParameterType.GetOrPost);
                requestR.AddParameter("mime_type", _fields.mime_type.ToString(), ParameterType.GetOrPost);
                requestR.AddParameter("json", _fields.json.ToString(), ParameterType.GetOrPost);
                var responseR = await clientR.ExecutePostAsync(requestR);
                JsonSerializerSettings jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                _respuesta = JsonConvert.DeserializeObject<cRespuesta>(responseR.Content, jsonSetting);
                if (_respuesta.status == "ERROR") { throw new Exception(_respuesta.message); }
            }
            catch (Exception e)
            {
                _respuesta = new cRespuesta();
                _respuesta.status = "ERROR";
                _respuesta.message = e.ToString();
                this._LOG.LogWriter("ERROR: Previo a llamada API" + (this._SERVER + "api.backend/setCounter") + ", DETALLE: " + e.ToString(), "");
            }
            return _respuesta;
        }
        public async Task<cRespuesta> sendStatusViaAPI(cStatus _status, string sDirLog)
        {
            cRespuesta _respuesta = new cRespuesta();
            try
            {
                var clientR = new RestClient((this._SERVER + "api.backend/setStatus"));
                var requestR = new RestRequest();
                requestR.Method = Method.Post;
                requestR.AddHeader("Content-Type", "application/json");
                requestR.AddParameter("id_file", _status.id_file.ToString(), ParameterType.GetOrPost);
                requestR.AddParameter("id_client", _status.id_client.ToString(), ParameterType.GetOrPost);
                requestR.AddParameter("filename", _status.filename.ToString(), ParameterType.GetOrPost);
                requestR.AddParameter("status", _status.status.ToString(), ParameterType.GetOrPost);
                requestR.AddParameter("created", _status.created.ToString("yyyy-MM-dd HH:mm:ss"), ParameterType.GetOrPost);
                var responseR = await clientR.ExecutePostAsync(requestR);
                JsonSerializerSettings jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                _respuesta = JsonConvert.DeserializeObject<cRespuesta>(responseR.Content, jsonSetting);
                if (_respuesta.status == "ERROR") { throw new Exception(_respuesta.message); }
            }
            catch (Exception e)
            {
                _respuesta = new cRespuesta();
                _respuesta.status = "ERROR";
                _respuesta.message = e.ToString();
                this._LOG.LogWriter("ERROR: Previo a llamada API" + (this._SERVER + "api.backend/setStatus") + ", DETALLE: " + e.ToString(), "");
            }
            return _respuesta;
        }
        public async Task<cRespuesta> sendPostViaAPI(cProfile _profile, string file, string sDirLog)
        {
            cRespuesta _respuesta = new cRespuesta();
			cPost _post = new cPost();
			bool _withError = false;
            try
            {
                Byte[] bytes = File.ReadAllBytes(file);
                _post.base64String = Convert.ToBase64String(bytes);
                _post.fullpath = file;
                _post.filename = Path.GetFileName(file);
                _post.id_client = _profile.id_client;
                _post.id_profile = _profile.id;
                _post.created = DateTime.Now;

                var clientR = new RestClient((this._SERVER + "api.backend/setPost"));
                var requestR = new RestRequest();
                requestR.RequestFormat = DataFormat.Json;
                requestR.Method = Method.Post;
                requestR.AddHeader("Content-Type", "application/json");
                requestR.AddJsonBody(JsonConvert.SerializeObject(_post));
                var responseR = await clientR.ExecutePostAsync(requestR);
                JsonSerializerSettings jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                _respuesta = JsonConvert.DeserializeObject<cRespuesta>(responseR.Content, jsonSetting);
                if (_respuesta == null)
                {
                    _withError = true;
                    _respuesta = new cRespuesta();
                    _respuesta.code = "9999";
                    _respuesta.message="Imposible realizar la operación solicitada";
                }
                else
                {
                    if (_respuesta.status == "ERROR") { _withError = (_respuesta.code.ToString() != "6003"); }
                }
                if (_withError) { throw new Exception(_respuesta.message); }
                switch (_profile.id_type_end) {
                    case 1: // borrar
                        File.Delete(file);
                        break;
                    case 2: // mover
                        File.Copy(file, _profile.post_unc_target + "\\" + _post.filename, true);
                        File.Delete(file);
                        break;
                }
            }
            catch (Exception e)
            {
                _respuesta = new cRespuesta();
                _respuesta.status = "ERROR";
                _respuesta.message = e.ToString();
                if (_withError) {
					File.Copy(file, _profile.post_unc_bad + "\\" + _post.filename, true);
					File.Delete(file);
				}
				this._LOG.LogWriter("ERROR: Previo a llamada API" + (this._SERVER + "api.backend/setPost") + ", DETALLE: " + e.ToString(), sDirLog);
            }
            return _respuesta;
        }

        private void DeleteEmptyDirs(string dir)
        {
            if (String.IsNullOrEmpty(dir))
                throw new ArgumentException(
                    "Starting directory is a null reference or an empty string",
                    "dir");

            try
            {
                foreach (var d in Directory.EnumerateDirectories(dir))
                {
                    DeleteEmptyDirs(d);
                }

                var entries = Directory.EnumerateFileSystemEntries(dir);

                if (!entries.Any())
                {
                    try
                    {
                        Directory.Delete(dir);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }
            }
            catch (UnauthorizedAccessException) { }
        }
    }
}
