using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebRole1.Models;
using Newtonsoft.Json;
using System.Text.Json;
using System.Dynamic;
using System.Linq;

namespace WebRole1.Controllers
{
    public class HomeController : Controller
    {
        BlobStorageService _blobStorageService = new BlobStorageService();
        // The cryptographic service provider
        private SHA256 sha256 = SHA256.Create();

        private static Random _random = new Random();

        private ICT2202ProjectEntities db = new ICT2202ProjectEntities();

        string id;

        bool corrupted = false;

        string server1 = "http://10.6.0.3:5000/";

        string server2 = "http://10.6.0.2:5000/";

        List<string> evidence_name = new List<string>();

        public ActionResult Index()
        {
            dynamic multiModel = new ExpandoObject();
            IEnumerable<Account> getUser = db.Accounts.SqlQuery("SELECT * FROM Account WHERE NOT First_Name = '" + (string)Session["First_Name"] + "';");
            ReceiveCase caseid = getusercase();
            if (corrupted)
            {
                return RedirectToAction("Contact");
            }
            multiModel.listUser = getUser;
            multiModel.listcase = caseid;
            ViewBag.Userlist = new MultiSelectList(listuser());
            ViewBag.Message = "Welcome " + (string)Session["First_Name"];
            return View(multiModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.myCountries = listuser();
            return View();
        }

        public List<string> listuser()
        {
            List<string> users = new List<string>();
            IEnumerable<Account> getUser = db.Accounts.SqlQuery("SELECT * FROM Account WHERE NOT First_Name = '" + (string)Session["First_Name"] + "';");
            foreach (var name in getUser)
            {
                users.Add(name.First_Name);
            }
            return users;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Home()
        {
            ViewBag.Message = "User";

            return View();
        }

        public ActionResult UploadCase() //Maybe don't need (Can delete) See How
        {
            CloudBlobContainer blobContainer = _blobStorageService.GetCloudBlobContainerCase();
            List<string> blobs = new List<string>();
            foreach (var blobItem in blobContainer.ListBlobs())
            {
                blobs.Add(blobItem.Uri.ToString());
            }
            return View(blobs);
        }

        [HttpPost]
        public ActionResult UploadCase(HttpPostedFileBase file) //Upload a new case to blob storage
        {
            List<Log> logs = new List<Log>();
            List<string> user_list = new List<string>();
            user_list.Add((string)Session["First_Name"]);
            var b_first = l_log("Upload", user_list);
            var b_second = l_log("AddUser", user_list);
            logs.Add(b_first);
            logs.Add(b_second);
            if (file.ContentLength > 0)
            {
                string random_id = RandomString(10);
                CloudBlobContainer blobContainer = _blobStorageService.GetCloudBlobContainerCase(random_id); //Create a new container (Folder)
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(file.FileName);
                blob.UploadFromStream(file.InputStream);
                string a = local_path("C:\\Users\\super\\Pictures\\ImageUploadTest\\" + file.FileName);
                //var b = l_log("Upload", user_list);
                var c = s_meta(a, file.FileName, Date_time(), Date_time());
                for(int i = 0; i < 2; i++)
                {
                    case_meta_data(random_id, c, logs[i]);
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult UploadEvidence()
        {
            ViewBag.DuplicateEvidence = "Please do not upload duplicate evidence";
            CloudBlobContainer blobContainer = _blobStorageService.GetCloudBlobContainer();
            List<string> blobs = new List<string>();
            foreach (var blobItem in blobContainer.ListBlobs())
            {
                blobs.Add(blobItem.Uri.ToString());
            }
            return View(blobs);
        }

        [HttpPost]
        public ActionResult UploadEvidence(HttpPostedFileBase file)
        {
            List<string> user_list = new List<string>();
            user_list.Add((string)Session["First_Name"]);
            if (file.ContentLength > 0)
            {
                CloudBlobContainer blobContainer = _blobStorageService.GetCloudBlobContainer((string)Session["case_id"]);
                List<string> checkName = GetEvidenceName(blobContainer);
                if (checkName.Contains(file.FileName) == false) // Means the file haven't been uploaded beforehand
                {
                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(file.FileName);
                    blob.UploadFromStream(file.InputStream);
                    string a = local_path("C:\\Users\\super\\Pictures\\ImageUploadTest\\" + file.FileName);
                    var b = l_log("Upload", user_list);
                    var c = s_meta(a, file.FileName, Date_time(), Date_time());
                    case_meta_data(Session["case_id"].ToString(), c, b);
                }
                else //Means Evidence has been uploaded before
                {
                    ViewBag.DuplicateEvidence = "Evidence has been uploaded before";
                    return View();
                }
            }
            //ViewBag.DuplicateEvidence = "Evidence has been successfully uploaded";
            return RedirectToAction("CaseInfo");
        }

        [HttpPost]
        public string DeleteImage(string Name)
        {
            Uri uri = new Uri(Name);
            string filename = System.IO.Path.GetFileName(uri.LocalPath);

            CloudBlobContainer blobContainer = _blobStorageService.GetCloudBlobContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(filename);

            blob.Delete();

            return "File Deleted.";
        }

        //Get local file path
        public static string local_path(string file)
        {
            var test = new HomeController();
            //string file_path = Path.GetFullPath(file);
            byte[] byte_stream = test.Hash_function(file);
            string hex_string = BytesToString(byte_stream);
            return hex_string;
        }

        private byte[] Hash_function(string filepath)
        {
            //Compute the file's hash
            using (FileStream stream = System.IO.File.OpenRead(filepath))
            {
                return sha256.ComputeHash(stream);
            }
        }

        //Return a byte array as a sequence of hex values.
        public static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("X2");
            return result;
        }

        public static string Date_time()
        {
            DateTime dateTime = DateTime.Now;
            return dateTime.ToString();
        }

        public static string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; //A...Z or a..z: length = 26

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString().ToLower();
        }

        public Meta_Data s_meta(string file_hash, string file_name, string m_date, string c_date)
        {
            var meta = new Meta_Data
            {
                File_Hash = file_hash,
                File_Name = file_name,
                Modified_Date = m_date,
                Creation_Date = c_date
            };
            return meta; 
        }

        public Log l_log(string action, List<string> username)
        {
            var log = new Log
            {
                Action = action,
                Username = username
            };
            return log;
        }

        public Pool case_meta_data(string random_caseid, Meta_Data meta_Data, Log log)
        {
            var sent = new Pool
            {
                case_id = random_caseid,
                meta_data = meta_Data,
                log = log
            };
            sendPool(sent);
            if (corrupted)
            {
                sendPool(sent);
            }
            return sent;
        }

        public void sendPool(Pool outside_Pool)
        {
            string result;
            string url;
            Outside_Pool outside = new Outside_Pool();
            List<Pool> pools = new List<Pool>();
            pools.Add(outside_Pool);
            outside.Pool = pools;
            string hello = JsonConvert.SerializeObject(outside);

            if (corrupted)
            {
                url = server2 + "receiveblock";
            }
            else
            {
                url = server1 + "receiveblock";
            }
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";
            httpRequest.Headers["Authorization"] = "Bearer secret-token-1";

            var data = hello;

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            if (result.Contains("Blockchain Verification Failed"))
            {
                corrupted = true;
                Console.WriteLine("Corrupted");
            }
            else
            {
                corrupted = false;
            }
        }

        public string requestCaseInfo(string case_id)
        {
            string url;
            string result;
            if (corrupted)
            {
                url = server2 + "caseinfo";
            }
            else
            {
                url = server1 + "caseinfo";
            }

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";
            httpRequest.Headers["Authorization"] = "Bearer secret-token-1";

            //var data = "{\"case_id\":" + "\"" + id + "\"}";
            var data = "{\"case_id\":" + "\"" + case_id + "\"}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            if (result.Contains("Blockchain Verification Failed"))
            {
                corrupted = true;
                Console.WriteLine("Corrupted");
            }
            else
            {
                corrupted = false;
            }
            return result;
        }//Get CaseInfo

        public ActionResult CaseInfo(string id)
        {
            string getcaseInfo = requestCaseInfo((string)Session["case_id"]);
            if (corrupted)
            {
                getcaseInfo = requestCaseInfo((string)Session["case_id"]);
                if (corrupted)
                {
                    return RedirectToAction("Contact");
                }
            }
            List<Block> block = new List<Block>();
            if(getcaseInfo.Contains("fail, cannot find case_id") != true)
            {
                Rootobject details = JsonConvert.DeserializeObject<Rootobject>(getcaseInfo);
                for (int i = 0; i < details.Blocks.Length; i++)
                {
                    block.Add(details.Blocks[i]);
                }
            }
            return View(block);
        } //Display CaseInfo back to View

        public ActionResult GetCaseID(string case_id)
        {
            Session["case_id"] = case_id;
            return RedirectToAction("CaseInfo");
        }

        public string usercase(string name) //Get all cases assigned to user
        {
            string url;
            string result;
            if (corrupted)
            {
                url = server2 + "usercase";
            }
            else
            {
                url = server1 + "usercase";
            }

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";
            httpRequest.Headers["Authorization"] = "Bearer secret-token-1";

            var data = "{\"Username\":" + "\"" + name + "\"}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            
            if (result.Contains("Blockchain Verification Failed"))
            {
                corrupted = true;
                Console.WriteLine("Corrupted");
            }
            else
            {
                corrupted = false;
            }
            return result;
        }

        public ReceiveCase getusercase() //Get usercase
        {
            List<string> number_cases = new List<string>();
            string checkvalue = usercase((string)Session["First_Name"]);
            if (corrupted)
            {
               checkvalue = usercase((string)Session["First_Name"]);
            }
            if(checkvalue.Contains("Blockchain Verification Failed"))
            {
                return null;
            }
            ReceiveCase details = JsonConvert.DeserializeObject<ReceiveCase>(checkvalue);
            return details;
        }

        public ActionResult AssignRemoveUser()
        {
            UserModel user = new UserModel();
            user.Users = PopulateUser();
            //ViewBag.Userlist = new MultiSelectList(getUsers,"AccountID", "FirstName");
            return View(user);
        }

        [HttpPost]
        public ActionResult AssignRemoveUser(UserModel user)
        {
            List<string> adduser = new List<string>();
            user.Users = PopulateUser();
            if (user.UserIds != null)
            {
                List<SelectListItem> selectedItems = user.Users.Where(p => user.UserIds.Contains(int.Parse(p.Value))).ToList();

                ViewBag.Message = "Selected Users:";
                foreach (var selectedItem in selectedItems)
                {
                    selectedItem.Selected = true;
                    ViewBag.Message += "\\n" + selectedItem.Text;
                    adduser.Add(selectedItem.Text);
                }
            }
            var a = s_meta("", "", "", "");
            var b = l_log("AddUser", adduser);
            case_meta_data(Session["case_id"].ToString(),a,b);
            return View(user);
        }

        private static List<SelectListItem> PopulateUser()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var db = new ICT2202ProjectEntities();
            var getUsers = db.Accounts.Select(c => new { AccountID = c.idAccount, FirstName = c.First_Name }).ToList();
            foreach (var it in getUsers)
            {
                items.Add(new SelectListItem
                {
                    Text = it.FirstName.ToString(),
                    Value = it.AccountID.ToString()
                });
            }


            return items;
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Accounts");
        }

        public ActionResult DownloadEvidence()
        {
            GetEvidence(); //Downloading Evidence
            Dictionary<string, string> blobFileNameHash = new Dictionary<string, string>();
            Dictionary<string, string> serverFileNameHash = new Dictionary<string, string>();
            var abc = GetFileNameHash();
            if (corrupted)
            {
                abc = GetFileNameHash();
                if (corrupted)
                {
                    return RedirectToAction("Contact");
                }
            }
            RootFileNameHash details = JsonConvert.DeserializeObject<RootFileNameHash>(abc);
            for(int i = 0; i < details.Files.Count();i++)
            {
                serverFileNameHash.Add(details.Files[i].File_name, details.Files[i].File_Hash);
            }
            for (int i = 0; i < evidence_name.Count; i++)
            {
                byte[] hashbyte = Hash_function("C:\\Users\\super\\Desktop\\DownloadBlob\\" + evidence_name[i]);
                string bytestring = BytesToString(hashbyte);
                blobFileNameHash.Add(evidence_name[i], bytestring);
            }
            if (checkError(serverFileNameHash,blobFileNameHash))
            {
                //Delete the files downloaded
                DeleteFiles();
                //ViewBag.DownloadError("Files corrupted. Will be changing server. Please try again");
                return RedirectToAction("CaseInfo");
            }
            //ViewBag.DownloadError("Download Completed");
            return View();
        }

        public void GetEvidence()
        {
            var abc = GetFileNameHash();
            List<string> downloadUser = new List<string>();
            downloadUser.Add((string)Session["First_Name"]);
            CloudBlobContainer blobContainer = _blobStorageService.GetCloudBlobContainer((string)Session["case_id"]);
            List<string> a = GetEvidenceName(blobContainer);
            foreach (var b in a)
            {
                evidence_name.Add(b);
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(b);
                System.IO.Directory.CreateDirectory("C:\\Users\\super\\Desktop\\DownloadBlob\\");
                FileStream file = System.IO.File.OpenWrite("C:\\Users\\super\\Desktop\\DownloadBlob\\" + b);
                blob.DownloadToStream(file);
                file.Close();
            }
            var c = s_meta("", "", Date_time(), Date_time());
            var d = l_log("DownloadEvidence", downloadUser);
            case_meta_data((string)Session["case_id"], c, d);
        }

        public string GetFileNameHash() //Get File Name and Hash from Blockchain
        {
            string result;
            string url;
            if (corrupted)
            {
                url = server2 + "filenameAndHash";
            }
            else
            {
                url = server1 + "filenameAndHash";
            }

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";
            httpRequest.Headers["Authorization"] = "Bearer secret-token-1";

            var data = "{\"case_id\":\"" + (string)Session["case_id"] + "\"}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            Console.WriteLine(httpResponse.StatusCode);
            if (result.Contains("Blockchain Verification Failed"))
            {
                corrupted = true;
                Console.WriteLine("Corrupted");
            }
            else
            {
                corrupted = false;
            }
            return result;
        }

        public bool checkError(Dictionary<string,string> server, Dictionary<string,string> blob) //Check if there are any file tempering
        {
            bool tampering = false;
            for(int i = 0; i < evidence_name.Count; i++)
            {
                if(server[evidence_name[i]] != blob[evidence_name[i]])
                {
                    tampering = true;
                }
            }
            return tampering;
        }

        public List<string> GetEvidenceName(CloudBlobContainer cloudBlobContainer) //Get Evidence Name List
        {
            var list = cloudBlobContainer.ListBlobs();
            List<string> EvidenceNamelist = list.OfType<CloudBlockBlob>().Select(b => b.Name).ToList();
            return EvidenceNamelist;
        }

        public void DeleteFiles()
        {
            try
            {
                for(int i = 0; i < evidence_name.Count; i++)
                {
                    if (System.IO.File.Exists(Path.Combine("C:\\Users\\super\\Desktop\\DownloadBlob\\",evidence_name[i])))
                    {
                        System.IO.File.Delete(Path.Combine("C:\\Users\\super\\Desktop\\DownloadBlob\\", evidence_name[i]));
                    }
                }
            }
            catch(IOException ioExp) 
            {
                Console.WriteLine(ioExp.Message);
            }
        }
    }
}