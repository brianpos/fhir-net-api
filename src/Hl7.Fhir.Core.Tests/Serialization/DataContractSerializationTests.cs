/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/ewoutkramer/fhir-net-api/master/LICENSE
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using Hl7.Fhir.Utils;
using System.Diagnostics;

namespace Hl7.Fhir.Tests.Serialization
{
    [TestClass]
#if PORTABLE45
	public class PortableDataContractSerializationTests
#else
    public class DataContractSerializationTests
#endif
    {
        [TestMethod]
        public void DataContractSerializeSpecificationZip()
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(Resource), new Type[] { typeof(Integer) });
            using (var archive = ZipFile.Open("../../../../Hl7.Fhir.Specification/specification.zip", ZipArchiveMode.Read))
            {
                var entries = archive.Entries.Where(e => Regex.IsMatch(e.Name, "^.*\\.xml$", RegexOptions.IgnoreCase));
                foreach (var zipArchiveEntry in entries.GetResources())
                {
                    var stream = Serialize(serializer, zipArchiveEntry);
                    var resource = Deserialize(serializer, stream);
                    string original_xml = FhirSerializer.SerializeResourceToXml(zipArchiveEntry);
                    string processed_xml = FhirSerializer.SerializeResourceToXml(resource);
                    Assert.AreEqual(original_xml, processed_xml);
                    stream.Dispose();
                }
            }
        }

        private static MemoryStream Serialize(DataContractSerializer serializer, Resource zipArchiveEntry)
        {
            var memoryStream = new MemoryStream();
            XmlDictionaryWriter binaryDictionaryWriter = XmlDictionaryWriter.CreateBinaryWriter(memoryStream);
            serializer.WriteObject(binaryDictionaryWriter, zipArchiveEntry);
            binaryDictionaryWriter.Flush();
            memoryStream.Position = 0;
            return memoryStream;
        }

        private static Resource Deserialize(DataContractSerializer serializer, Stream stream)
        {
            stream.Position = 0;
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            var xmlDictionaryReaderQuotas = new XmlDictionaryReaderQuotas();
            xmlDictionaryReaderQuotas.MaxArrayLength = int.MaxValue;
            xmlDictionaryReaderQuotas.MaxDepth = int.MaxValue;
            xmlDictionaryReaderQuotas.MaxStringContentLength = int.MaxValue;
            XmlDictionaryReader binaryDictionaryReader = XmlDictionaryReader.CreateBinaryReader(bytes, xmlDictionaryReaderQuotas);
            var resource = (Resource)serializer.ReadObject(binaryDictionaryReader, true);

            return resource;
        }

        [TestMethod]
        public void BinarySerializeBundleStuctureDefs()
        {
            // this will create a custom index and file.
            using (var archive = ZipFile.Open("../../../../Hl7.Fhir.Specification/specification.zip", ZipArchiveMode.Read))
            {
                string outputfile = @"c:\temp\binary_bindle.fbin";
                List<IndexItem> items = new List<IndexItem>();
                using (FileStream fs = new FileStream(outputfile, FileMode.Create, FileAccess.Write))
                {
                    var entries = archive.Entries.Where(e => Regex.IsMatch(e.Name, "^.*\\.xml$", RegexOptions.IgnoreCase));
                    foreach (var zipArchiveEntry in entries.GetResources())
                    {
                        long lastPosition = fs.Position;
                        byte[] original_xml = FhirSerializer.SerializeResourceToXmlBytes(zipArchiveEntry);
                        GZipStream zs = new GZipStream(fs, CompressionMode.Compress, true);
                        using (zs)
                        {
                            zs.Write(original_xml, 0, original_xml.Length);
                        }
                        var item = new IndexItem(zipArchiveEntry.ResourceType.ToString(), lastPosition, fs.Position - lastPosition);
                        items.Add(item);
                    }
                    // Now serialize the tuple array
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(items.GetType());
                    long posStartIndex = fs.Position;
                    GZipStream zsIndex = new GZipStream(fs, CompressionMode.Compress, true);
                    using (zsIndex)
                    {
                        xs.Serialize(zsIndex, items);
                    }
                    long posEndIndex = fs.Position;
                    BinaryWriter sw2 = new BinaryWriter(fs);
                    sw2.Write(posStartIndex);
                    sw2.Write(posEndIndex);
                }

                System.Diagnostics.Trace.WriteLine(String.Format("File length: {0}, resources: {1}", new System.IO.FileInfo(outputfile).Length, items.Count));
                foreach (var item in items)
                {
                    System.Diagnostics.Trace.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}",
                        items.IndexOf(item), item.ResourceType, item.Start, item.Length));
                }

                // Now randomly read one from the middle somewhere
                Stopwatch sw = new Stopwatch();
                sw.Start();
                System.IO.FileStream fsRead = new FileStream(outputfile, FileMode.Open, FileAccess.Read);
                var itemRead = items[7082];
                byte[] contentCompressedContent = new byte[itemRead.Length];
                fsRead.Seek(itemRead.Start, SeekOrigin.Begin);
                fsRead.Read(contentCompressedContent, 0, (int)itemRead.Length);
                MemoryStream ms = new MemoryStream(contentCompressedContent);
                GZipStream zs2 = new GZipStream(ms, CompressionMode.Decompress);
                var rdr = SerializationUtil.XmlReaderFromStream(zs2);
                var parser = new FhirXmlParser();
                Resource result = parser.Parse<Resource>(rdr);
                System.Diagnostics.Trace.WriteLine(String.Format("SeekAndRead: {0}", sw.Elapsed.TotalSeconds));
            }
        }

        public class IndexItem
        {
            public IndexItem()
            {
            }

            public IndexItem(string resourceType, long start, long length)
            {
                ResourceType = resourceType;
                Start = start;
                Length = length;
            }

            public string ResourceType { get; set; }
            public long Start { get; set; }
            public long Length { get; set; }
        }

        [TestMethod]
        public void BinarySerializeBundleStuctureDefsReadOnly()
        {
            string outputfile = @"c:\temp\binary_bindle.fbin";
            System.IO.FileStream fsRead = new FileStream(outputfile, FileMode.Open, FileAccess.Read);

            // read the index from the file
            List<IndexItem> items = new List<IndexItem>();
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(items.GetType());
            fsRead.Seek(-2 * sizeof(long), SeekOrigin.End);
            BinaryReader sr = new BinaryReader(fsRead);
            long posStartIndex = sr.ReadInt64();
            long posEndIndex = sr.ReadInt64();
            fsRead.Seek(posStartIndex, SeekOrigin.Begin);
            GZipStream zs = new GZipStream(fsRead, CompressionMode.Decompress, true);
            using (zs)
            {
                items = (List<IndexItem>)xs.Deserialize(zs);
            }

            // Now randomly read one from the middle somewhere
            // Test 1
            {

                Stopwatch sw = new Stopwatch();
                sw.Start();
                var itemRead = items[7082]; // new IndexItem("ValueSet", 6055139, 21491);// 6067564, 21508);
                Resource result = ReadResourceAtPosition(outputfile, itemRead);
                System.Diagnostics.Trace.WriteLine(String.Format("SeekAndRead: {0}", sw.Elapsed.TotalSeconds));
            }

            // Test 2
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var itemRead = items[7082];
                Resource result = ReadResourceAtPosition(outputfile, itemRead);
                System.Diagnostics.Trace.WriteLine(String.Format("SeekAndRead: {0} {1}", sw.Elapsed.TotalSeconds, result.ResourceType.ToString()));
            }

            // Test 3
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var itemRead = items[7053];
                Resource result = ReadResourceAtPosition(outputfile, itemRead);
                System.Diagnostics.Trace.WriteLine(String.Format("SeekAndRead: {0}", sw.Elapsed.TotalSeconds));
            }

            // Test 4
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                var itemRead = items[6932];
                Resource result = ReadResourceAtPosition(outputfile, itemRead);
                sw.Stop();
                System.Diagnostics.Trace.WriteLine(String.Format("SeekAndRead: {0}, {1} composed concepts in the resource", sw.Elapsed.TotalSeconds, (result as ValueSet).Compose.Include.First().Concept.Count));
            }
        }

        private static Resource ReadResourceAtPosition(string outputfile, IndexItem itemRead)
        {
            System.IO.FileStream fsRead = new FileStream(outputfile, FileMode.Open, FileAccess.Read);
            byte[] contentCompressedContent = new byte[itemRead.Length];
            fsRead.Seek(itemRead.Start, SeekOrigin.Begin);
            int nRead = fsRead.Read(contentCompressedContent, 0, (int)itemRead.Length);
            MemoryStream ms = new MemoryStream(contentCompressedContent);
            GZipStream zs2 = new GZipStream(ms, CompressionMode.Decompress);
            var rdr = SerializationUtil.XmlReaderFromStream(zs2);
            var parser = new FhirXmlParser();
            Resource result = parser.Parse<Resource>(rdr);
            return result;
        }

        [TestMethod]
        public void DataContractSerializerCompareTest()
        {
            string examplesZip = @"TestData\examples.zip";

            FhirXmlParser parser = new FhirXmlParser();
            DataContractSerializer serializer = new DataContractSerializer(typeof(Resource));

            var xmlDictionaryReaderQuotas = new XmlDictionaryReaderQuotas();
            xmlDictionaryReaderQuotas.MaxArrayLength = int.MaxValue;
            xmlDictionaryReaderQuotas.MaxDepth = int.MaxValue;
            xmlDictionaryReaderQuotas.MaxStringContentLength = int.MaxValue;

            var sw = new Stopwatch();
            sw.Start();

            int errorCount = 0;
            int testFileCount = 0;
            var zip = ZipFile.OpenRead(examplesZip);
            using (zip)
            {
                foreach (var entry in zip.Entries)
                {
                    Stream file = entry.Open();
                    using (file)
                    {
                        testFileCount++;


                        // This test will verify the time taken to just read the content into a memory stream
                        // (no processing)
                        using (MemoryStream memStream = new MemoryStream())
                        {
                            file.CopyTo(memStream);
                            memStream.Seek(0, SeekOrigin.Begin);
                            Resource resourceFhirXml = null;

                            using (var reader = SerializationUtil.WrapXmlReader(XmlReader.Create(memStream)))
                            {
                                resourceFhirXml = parser.Parse<Resource>(reader);
                            }

                            memStream.Seek(0, SeekOrigin.Begin);
                            XmlDictionaryReader binaryDictionaryReader = XmlDictionaryReader.CreateBinaryReader(memStream, xmlDictionaryReaderQuotas);
                            var resource = (Resource)serializer.ReadObject(binaryDictionaryReader, true);
                        }

                        //using (var reader = SerializationUtil.WrapXmlReader(XmlReader.Create(file)))
                        //{
                        //    var resourceFhirXml = parser.Parse<Resource>(reader);
                        //}
                    }
                }
            }

            sw.Stop();

            Debug.WriteLine(sw.ElapsedMilliseconds);
            Assert.IsTrue(sw.ElapsedMilliseconds < 26000);

            // Assert.IsTrue(140 >= errorCount, String.Format("Failed search parameter data extraction, missing data in {0} of {1} search parameters", missingSearchValues.Count(), exampleSearchValues.Count));
        }
    }
}
