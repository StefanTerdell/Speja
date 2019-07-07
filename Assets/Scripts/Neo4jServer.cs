using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class Neo4jServer : MonoBehaviour
{
    public string Username = "";
    public string Password = "";
    public string IP = "localhost:17474";
    public bool DebugLog = false;
    public int Limit = 1000;

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
            var wreq = WebRequest.Create("http://" + IP + "/db/data/transaction/commit");
            wreq.Method = "POST";
            wreq.Credentials = new NetworkCredential(Username, Password);

            var requestStream = new StreamWriter(wreq.GetRequestStream());
            requestStream.Write("{\"statements\" : [ { \"statement\" : \"" + query + " LIMIT " + Limit + "\", \"resultDataContents\" : [ \"graph\" ] } ]}");

            requestStream.Flush();
            requestStream.Close();

            var wres = wreq.GetResponse();
            var stream = wres.GetResponseStream();
            var streamReader = new StreamReader(stream);
            var responseJson = streamReader.ReadToEnd();

            streamReader.Close();
            stream.Close();

            if (DebugLog)
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

    public class RootObject
    {
        public List<Result> results;
        public List<object> errors;

        public class Result
        {
            public List<string> columns;
            public List<GraphData> data;

            public class GraphData
            {
                public Graph graph;

                public class Graph
                {
                    public List<Node> nodes;
                    public List<Relationship> relationships;

                    public class Node
                    {
                        public string id;
                        public List<string> labels;
                        public dynamic properties;
                    }

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