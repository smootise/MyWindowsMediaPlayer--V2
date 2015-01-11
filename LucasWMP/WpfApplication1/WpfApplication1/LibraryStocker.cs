using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace WpfApplication1
{
    class LibraryStocker
    {
        protected List<string> listMusic;
        protected List<string> listPic;
        protected List<string> listVideo;
        protected bool         isLoad;

        public enum FileType
        {
            Video,
            Picture,
            Music,
            Other
        }

        public LibraryStocker()
        {
            isLoad = false;
        }

        public bool  loadFromXml()
        {
            if (isLoad == true)
                return (true);
            XmlSerializer SerializerObj = new XmlSerializer(typeof(List<string>));
            FileStream ReadFileStream1 = new FileStream("./ListMusic.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
            FileStream ReadFileStream2 = new FileStream("./ListPicture.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
            FileStream ReadFileStream3 = new FileStream("./ListVideo.txt", FileMode.Open, FileAccess.Read, FileShare.Read);

            listMusic = (List<string>)SerializerObj.Deserialize(ReadFileStream1);
            listPic = (List<string>)SerializerObj.Deserialize(ReadFileStream2);
            listVideo = (List<string>)SerializerObj.Deserialize(ReadFileStream3);

            ReadFileStream1.Close();
            ReadFileStream2.Close();
            ReadFileStream3.Close();
            isLoad = true;
            return (isLoad);
        }

        public List<string> getMusic()
        {
            if (isLoad == false)
                loadFromXml();
            return (listMusic);
        }

        public List<string> getPic()
        {
            if (isLoad == false)
                loadFromXml();
            return (listPic);
        }

        public List<string> getVideo()
        {
            if (isLoad == false)
                loadFromXml();
            return (listVideo);
        }

        public static FileType getFileType(string param)
        {
            string[] extVideo = { ".mp4" };
            string[] extPic = { ".jpg" };
            string[] extMusic = { ".mp3" };

            for (int i = 0; i < extVideo.Length; i++)
                if (extVideo[i] == param.Substring(param.LastIndexOf('.')))
                    return (FileType.Video);
            for (int i = 0; i < extPic.Length; i++)
                if (extPic[i] == param.Substring(param.LastIndexOf('.')))
                    return (FileType.Picture);
            for (int i = 0; i < extMusic.Length; i++)
                if (extMusic[i] == param.Substring(param.LastIndexOf('.')))
                    return (FileType.Music);
            return (FileType.Other);
        }

        protected void updateXml(FileType param)
        {
            XmlSerializer SerializerObj = new XmlSerializer(typeof(List<string>));
            TextWriter WriteFileStream;

            if (param == FileType.Other)
                return ;
            if (param == FileType.Music)
            {
                WriteFileStream = new StreamWriter("./ListMusic.txt");
                SerializerObj.Serialize(WriteFileStream, listMusic);
            }
            else if (param == FileType.Video)
            {
                WriteFileStream = new StreamWriter("./ListVideo.txt");
                SerializerObj.Serialize(WriteFileStream, listVideo);
            }
            else
            {
                WriteFileStream = new StreamWriter("./ListPicture.txt");
                SerializerObj.Serialize(WriteFileStream, listPic);
            }
            WriteFileStream.Close();
        }

        public void addMedia(string param)
        {
            FileType fileType = getFileType(param);

            if (isLoad == false)
                loadFromXml();
            if (fileType == FileType.Video && listVideo.Contains(param) == false)
                   listVideo.Add(param);
            if (fileType == FileType.Picture && listPic.Contains(param) == false)
                    listPic.Add(param);
            if (fileType == FileType.Music && listMusic.Contains(param) == false)
                    listMusic.Add(param);
            updateXml(fileType);
        }
    }
}
