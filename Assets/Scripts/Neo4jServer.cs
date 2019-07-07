using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class Neo4jServer : MonoBehaviour
{
    public string _username = "";
    public string _password = "";
    public string _host = "localhost:17474";
    public bool _logResponse = false;
    public int _limit = 1000;

    static Neo4jServer instance;

    private void Awake()
    {
        instance = this;
    }

    public static string Query(string query)
    {
        return instance.GetQuery(query);
    }

    string GetQuery(string query)
    {
        try
        {
            var wreq = WebRequest.Create($"http://{_host}/db/data/transaction/commit");
            wreq.Method = "POST";
            wreq.Credentials = new NetworkCredential(_username, _password);

            var requestStream = new StreamWriter(wreq.GetRequestStream());
            requestStream.Write("{\"statements\" : [ { \"statement\" : \"" + query + " LIMIT " + _limit + "\", \"resultDataContents\" : [ \"graph\" ] } ]}");

            requestStream.Flush();
            requestStream.Close();

            var wres = wreq.GetResponse();
            var stream = wres.GetResponseStream();
            var streamReader = new StreamReader(stream);
            var responseJson = streamReader.ReadToEnd();

            streamReader.Close();
            stream.Close();

            if (_logResponse)
            {
                Debug.Log("Request Headers:" + wreq.Headers);
                Debug.Log("Response Json:" + responseJson);
            }

            return responseJson;
        }
        catch (WebException webex)
        {
            Debug.LogError("neo4j connection failed.\nReason:" + webex.Message);

            return "{}";
        }
    }

    public static RootObject QueryObject(string query)
    {
        return instance.GetQueryObject(query);
    }

    public RootObject GetQueryObject(string query)
    {
        return JsonUtility.FromJson<RootObject>(GetQuery(query));
    }

    [System.Serializable]
    public class RootObject
    {
        public List<Result> results;
        public List<object> errors;

        [System.Serializable]
        public class Result
        {
            public List<string> columns;
            public List<GraphData> data;

            [System.Serializable]
            public class GraphData
            {
                public Graph graph;

                [System.Serializable]
                public class Graph
                {
                    public List<Node> nodes;
                    public List<Relationship> relationships;

                    [System.Serializable]
                    public class Node
                    {
                        public string id;
                        public List<string> labels;
                        public dynamic properties;
                    }

                    [System.Serializable]
                    public class Relationship
                    {
                        public string id;
                        public string type;
                        public string startNode;
                        public string endNode;
                        public dynamic properties;
                    }
                }
            }
        }
    }
}