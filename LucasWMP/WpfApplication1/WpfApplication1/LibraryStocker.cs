using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;


class LibraryStocker
{
    protected List<string> listMusic = new List<string>();
    protected List<string> listPic = new List<string>();
    protected List<string> listVideo = new List<string>();
    protected bool         isLoad;
    string                 filterStr;

    public enum Filter
    {
        None,
        Name,
        Album,
        Artist,
        Genre,
        MissingNo
    }
    public Filter filter;

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
        filterStr = "";
        filter = Filter.None;
    }

    public void setFilter(string paramStr, Filter paramFilter = Filter.MissingNo)
    {
        if (paramFilter != Filter.MissingNo)
            filter = paramFilter;
        filterStr = paramStr;
    }

    public Filter getFilter()
    {
        return filter;
    }

    public List<string> getPlayList()
    {
        List<string> list = new List<string>();
        XmlSerializer SerializerObj = new XmlSerializer(typeof(List<string>));

        try
        {
            FileStream ReadFileStream = new FileStream("ListPlayList.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
            var tmp = (List<string>)SerializerObj.Deserialize(ReadFileStream);
            list.AddRange(tmp);
            ReadFileStream.Close();
        }
        catch (Exception e)
        {
            
        }
        return (list);
    }

    public void  majPlayList(List<string> list)
    {
        XmlSerializer SerializerObj = new XmlSerializer(typeof(List<string>));

        try
        {
            TextWriter WriteFileStream = new StreamWriter("ListPlayList.txt");
            SerializerObj.Serialize(WriteFileStream, list);
            WriteFileStream.Close();
        }
        catch (Exception e)
        {

        }
    }

    public bool  loadFromXml()
    {
        if (isLoad == true)
            return (true);
        XmlSerializer SerializerObj = new XmlSerializer(typeof(List<string>));

        try
        {
            FileStream ReadFileStream1 = new FileStream("ListMusic.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
            FileStream ReadFileStream2 = new FileStream("ListPicture.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
            FileStream ReadFileStream3 = new FileStream("ListVideo.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
            var tmp1 = (List<string>)SerializerObj.Deserialize(ReadFileStream1);
            var tmp2 = (List<string>)SerializerObj.Deserialize(ReadFileStream2);
            var tmp3 = (List<string>)SerializerObj.Deserialize(ReadFileStream3);
            listMusic.AddRange(tmp1);
            listPic.AddRange(tmp2);
            listVideo.AddRange(tmp3);
            ReadFileStream1.Close();
            ReadFileStream2.Close();
            ReadFileStream3.Close();
        }
        catch (Exception e)
        {
            isLoad = true;
            return (false);
        }
        isLoad = true;
        return (isLoad);
    }

    protected List<string> applyFilter(List<string> toApplyTo)
    {
        if (filter == Filter.None)
            return (toApplyTo);
        List<string> tmp = new List<string>();
        tmp.AddRange(toApplyTo);
        for (int i = 0; i < tmp.Count(); i++)
        {
            TagLib.File file = TagLib.File.Create(tmp.ElementAt(i));
            string toCmp = "";
            if (filter == Filter.Name)
                toCmp = file.Tag.Title;
            else if (filter == Filter.Album)
                toCmp = file.Tag.Album;
            else if (filter == Filter.Artist)
                toCmp = file.Tag.Artist;
           // else if (filter == Filter.Genre)
           //     toCmp = file.Tag.Genre;
            if (toCmp.Contains(filterStr) == false)
                tmp.Remove(listVideo.ElementAt(i));
        }
        return (tmp);
    }

    public List<string> getMusic()
    {
        if (isLoad == false)
            loadFromXml();
        return (applyFilter(listMusic));
    }

    public List<string> getPic()
    {
        if (isLoad == false)
            loadFromXml();
        return (applyFilter(listPic));
    }

    public List<string> getVideo()
    {
        if (isLoad == false)
            loadFromXml();
        return (applyFilter(listVideo));
    }

    public static FileType getFileType(string param)
    {
        string[] extVideo = { ".mp4", ".avi", ".wmv", ".mkv" };
        string[] extPic = { ".jpg", ".bmp", ".dib", ".jpeg", ".jpe", ".gif", ".png"};
        string[] extMusic = { ".mp3", ".wma", ".wav", ".ogg", ".m4p", ".3gp"};

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
        try
        {
            if (param == FileType.Music)
            {
                WriteFileStream = new StreamWriter("ListMusic.txt");
                SerializerObj.Serialize(WriteFileStream, listMusic);
            }
            else if (param == FileType.Video)
            {
                WriteFileStream = new StreamWriter("ListVideo.txt");
                SerializerObj.Serialize(WriteFileStream, listVideo);
            }
            else
            {
                WriteFileStream = new StreamWriter("ListPicture.txt");
                SerializerObj.Serialize(WriteFileStream, listPic);
            }
            WriteFileStream.Close();
        }
        catch (Exception e)
        {

        }
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

    public void removeMedia(string param)
    {
        FileType fileType = getFileType(param);

        if (fileType == FileType.Other)
            return;

        if (fileType == FileType.Video)
        { 
            for (int i = 0; i < listVideo.Count(); i++)
                if (listVideo.ElementAt(i).Contains(param))
                    listVideo.Remove(listVideo.ElementAt(i));
        }
        else if (fileType == FileType.Music)
        {
            for (int i = 0; i < listMusic.Count(); i++)
                if (listMusic.ElementAt(i).Contains(param))
                    listMusic.Remove(listMusic.ElementAt(i));
        }
        else if (fileType == FileType.Picture)
        {
            for (int i = 0; i < listPic.Count(); i++)
                if (listPic.ElementAt(i).Contains(param))
                    listPic.Remove(listPic.ElementAt(i));
        }
        updateXml(fileType);
    }

    public string   getfullpath(string param)
    {
        FileType fileType = getFileType(param);

        if (fileType == FileType.Other)
            return ("error");

        if (fileType == FileType.Video)
        {
            for (int i = 0; i < listVideo.Count(); i++)
                if (listVideo.ElementAt(i).Contains(param))
                    return (listVideo.ElementAt(i));
        }
        else if (fileType == FileType.Music)
        {
            for (int i = 0; i < listMusic.Count(); i++)
                if (listMusic.ElementAt(i).Contains(param))
                    return (listMusic.ElementAt(i));
        }
        else if (fileType == FileType.Picture)
        {
            for (int i = 0; i < listPic.Count(); i++)
                if (listPic.ElementAt(i).Contains(param))
                    return (listPic.ElementAt(i));
        }
        return ("error");
    }
}
