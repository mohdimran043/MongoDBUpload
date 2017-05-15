using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoUpload
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("MurasalatRepository");
            var fs = new GridFSBucket(database);

            var id = UploadFile(fs);

            DownloadFile(fs, id);
        }

        private static ObjectId UploadFile(GridFSBucket fs)
        {
            using (var s = File.OpenRead(@"C:\Users\Mohd\Documents\visual studio 2013\Projects\MongoUpload\MongoUpload\App_Data\multipage_tif_example.tif"))
            {
                var t = Task.Run<ObjectId>(() =>
                {
                    return
                        fs.UploadFromStreamAsync("multipage_tif_example.tif", s);
                });

                return t.Result;
            }
        }

        private static void DownloadFile(GridFSBucket fs, ObjectId id)
        {
            //This works
            var t = fs.DownloadAsBytesByNameAsync("multipage_tif_example.tif");
            Task.WaitAll(t);
            var bytes = t.Result;


            //This blows chunks (I think it's a driver bug, I'm using 2.1 RC-0)
            var x = fs.DownloadAsBytesAsync(id);
            Task.WaitAll(x);
        }
    }
}
