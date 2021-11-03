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

        public ActionResult Index()
        {
            dynamic multiModel = new ExpandoObject();
            IEnumerable<Account> getUser = db.Accounts.SqlQuery("SELECT * FROM Account WHERE NOT Username = '" + (string)Session["username"] + "';");
            ReceiveCase caseid = getusercase();
            multiModel.listUser = getUser;
            multiModel.listcase = caseid;
            ViewBag.Userlist = new MultiSelectList(listuser());
            ViewBag.Message = "Welcome " + (string)Session["username"];
            if (multiModel.listcase != null) //Remove caseid as verification
            {
                return View(multiModel);
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.myCountries = listuser();
            return View();
        }

        public List<string> listuser()
        {
            Dictionary<int, string> keyValues = new Dictionary<int, string>();
            List<string> users = new List<string>();
            IEnumerable<Account> getUser = db.Accounts.SqlQuery("SELECT * FROM Account WHERE NOT Username = '" + (string)Session["username"] + "';");
            foreach (var name in getUser)
            {
                //keyValues.Add(name.idAccount, name.First_Name);
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
            List<string> user_list = new List<string>();
            user_list.Add((string)Session["username"]);
            if (file.ContentLength > 0)
            {
                string random_id = RandomString(10);
                CloudBlobContainer blobContainer = _blobStorageService.GetCloudBlobContainerCase(random_id); //Create a new container (Folder)
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(file.FileName);
                blob.UploadFromStream(file.InputStream);
                string a = local_path(file.FileName);
                var b = l_log("Upload", user_list);
                var c = s_meta(a, file.FileName, Date_time(), Date_time());
                case_meta_data(random_id, c, b);
            }
            return RedirectToAction("Index");
        }

        public ActionResult UploadEvidence()
        {
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
            user_list.Add((string)Session["username"]);
            if (file.ContentLength > 0)
            {
                CloudBlobContainer blobContainer = _blobStorageService.GetCloudBlobContainer((string)Session["case_id"]);
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(file.FileName);
                blob.UploadFromStream(file.InputStream);
                string a = local_path(file.FileName);
                var b = l_log("Upload",user_list);
                var c = s_meta(a,file.FileName, Date_time(),Date_time());
                case_meta_data(RandomString(10),c, b);
            }
            return RedirectToAction("UploadEvidence");
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
            byte[] byte_stream = test.Hash_function("C:\\Users\\super\\Pictures\\ImageUploadTest\\" + file);
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
            DateTime now = new DateTime();
            return now.ToString();
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

        public static Pool case_meta_data(string random_caseid, Meta_Data meta_Data, Log log)
        {
            var sent = new Pool
            {
                case_id = random_caseid,
                meta_data = meta_Data,
                log = log
            };
            sendPool(sent);
            return sent;
        }

        public static void sendPool(Pool outside_Pool)
        {
            Outside_Pool outside = new Outside_Pool();
            List<Pool> pools = new List<Pool>();
            pools.Add(outside_Pool);
            outside.Pool = pools;
            string hello = JsonConvert.SerializeObject(outside);

            var url = "http://192.168.50.253:5000/receiveblock";

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
                var result = streamReader.ReadToEnd();
            }

            Console.WriteLine(httpResponse.StatusCode);
        }

        public static string requestCaseInfo(string case_id)
        {
            string result;
            var url = "http://192.168.50.253:5000/caseinfo";

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
            return result;
        }//Get CaseInfo

        public ActionResult CaseInfo(string id)
        {
            string getcaseInfo = requestCaseInfo((string)Session["case_id"]);
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

        public static string usercase(string username) //Get all cases assigned to user
        {
            string result;
            var url = "http://192.168.50.253:5000/usercase";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";
            httpRequest.Headers["Authorization"] = "Bearer secret-token-1";

            var data = "{\"Username\":" + "\"" + username + "\"}";

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
            return result;
        }

        public ReceiveCase getusercase() //Get usercase
        {
            List<string> number_cases = new List<string>();
            string checkvalue = usercase((string)Session["username"]);
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
            //case_meta_data(RandomString(10),a ,b);

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
    }
}