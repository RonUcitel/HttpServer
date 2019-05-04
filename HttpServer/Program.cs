using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace HttpServer
{
    class Program
    {
        const string Url = "http://localhost:8080/";
        static void Main(string[] args)
        {
            SimpleListenerExample(new string[] { Url });
            Console.Read();
        }

        static void SimpleListenerExample(string[] prefixes)
        {
            //check if the HttpListener is supported
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }


            //check all prefixes
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");


            // Create a listener.
            HttpListener listener = new HttpListener();


            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }

            //start listening
            listener.Start();

            //get into the listening commands: "the code"
            Console.WriteLine("Listening...");
            while (true)
            {
                HttpLoopContent(ref listener);
            }
            listener.Stop();
        }

        static void HttpLoopContent(ref HttpListener listener)
        {
            //geting the request
            // Note: The GetContext method blocks while waiting for a request. 
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            string urlCont = request.RawUrl.Remove(0, 1);

            Console.WriteLine(urlCont);
            //Console.WriteLine(GetPathfromUrlCont(urlCont));
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            //send the webpage

            if (Redirect(request, response, Url, Url + "c:/"))
            {
                Console.WriteLine("The Site has redirected from {0} to {1}", Url, Url + "c:/");
            }
            else if (request.AcceptTypes[0] == "text/html" && Directory.Exists(GetPathfromUrlCont(urlCont)))
            {
                //if the request is of an HTML body
                //and the continue of the URL is a directory
                SendHtmlRespond(response, GetPathfromUrlCont(urlCont));
            }
            else if (request.AcceptTypes[0] == "image/webp" && urlCont == "favicon.ico")
            {

            }
            else if (request.AcceptTypes[0] == "text/html" && File.Exists(GetPathfromUrlCont(urlCont)))
            {
                //if the request is of an HTML body
                //and the continue of the URL is a file
                SendFile(response, GetPathfromUrlCont(urlCont));
            }
            else
            {
                //send 404 message
            }
        }

        static void SendHtmlRespond(HttpListenerResponse response, string path)
        {
            // Construct a response.

            string responseString = "<HTML><BODY><title>" + path + "</title>";

            //enter all the Directories:
            string[] names = GetDirectoriesNamesInPath(path);
            for (int i = 0; i < names.Length; i++)
            {
                responseString += "<br><br>";
                responseString += GetLinkedText(names[i], Url + path + names[i]);
            }

            names = GetFilesNamesInPath(path);
            for (int i = 0; i < names.Length; i++)
            {
                responseString += "<br><br>";
                responseString += GetLinkedText(names[i], Url + path + names[i]);
            }

            responseString += "</BODY></HTML>";

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }

        static bool Redirect(HttpListenerRequest request, HttpListenerResponse response, string from, string to)
        {
            if (request.Url.OriginalString == from)
            {
                // Sets the location header, status code and status description.
                response.Redirect(to);
                response.Close();
                return true;
            }
            return false;
        }
        static void SendFile(HttpListenerResponse response, string fullFilePath)
        {
            var fileStream = File.OpenRead(fullFilePath);
            response.ContentType = "application/octet-stream";
            response.ContentLength64 = (new FileInfo(fullFilePath)).Length;
            response.AddHeader("Content-Disposition", "Attachment; filename=\"" + Path.GetFileName(fullFilePath) + "\"");
            fileStream.CopyTo(response.OutputStream);
            response.OutputStream.Close();
        }

        static string[] GetDirectoriesNamesInPath(string path)
        {
            string[] names = Directory.GetDirectories(path);
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = names[i].Remove(0, path.Length);
            }
            return names;
        }
        static string[] GetFilesNamesInPath(string path)
        {
            string[] names = Directory.GetFiles(path);
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = names[i].Remove(0, path.Length);
            }
            return names;
        }
        static string GetLinkedText(string text, string link)
        {
            return "<a href=" + '"' + link + '"' + ">" + text + "</a>";
        }
        static string GetPathfromUrlCont(string urlCont)
        {
            urlCont = urlCont.Replace("%20", " ");
            string[] parts = urlCont.Split('/');
            urlCont = "";
            for (int i = 0; i < parts.Length; i++)
            {
                urlCont += parts[i] + @"\";
            }
            urlCont = urlCont.Remove(urlCont.Length - 1);
            return urlCont;
        }
    }
}
