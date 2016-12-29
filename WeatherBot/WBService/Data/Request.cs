using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WBService.Data
{
    public class Request
    {
        public string ToUserName { get; set; } = "";
        public string FromUserName { get; set; } = "";
        public string CreateTime { get; set; } = "";

        public string MsgType { get; set; } = "";
        public string Content { get; set; } = "";
        public string MsgId { get; set; } = "";
    }
    
    public class Response
    {
        public string ToUserName { get; set; } = "";
        public string FromUserName { get; set; } = "";
        public string CreateTime { get; set; } = "";

        public string MsgType { get; set; } = "";
        public string Content { get; set; } = "";
    }
    
    /*
    public class RichMsg
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PicUrl { get; set; }
        public string Url { get; set; }
    }
    
    public class XmlMsgNode
    {
        protected XmlDocument Doc;
        protected XmlNode Node;

        //
        // Read Only
        //

        protected XmlMsgNode(XmlDocument doc, XmlNode node)
        {
            Doc = doc;
            Node = node;
        }

        public bool HasParameter(string param)
        {
            return Node[param] != null;
        }

        protected static string GetNodeChildInnerText(XmlNode node, string param)
        {
            if (node[param] != null)
            {
                return node[param].InnerText;
            }
            else
            {
                return null;
            }
        }

        public string GetParameter(string param)
        {
            return GetNodeChildInnerText(Node, param);
        }

        public XmlMsgNode GetParamNode(string param, bool createIfUnexist = false)
        {
            if (Node[param] != null)
            {
                return new XmlMsgNode(Doc, Node[param]);
            }
            else
            {
                if (createIfUnexist)
                {
                    var node = Doc.CreateElement(param);
                    Node.AppendChild(node);

                    return new XmlMsgNode(Doc, node);
                }
                else
                {
                    return null;
                }
            }
        }

        //
        // Update
        //
        protected XmlMsgNode()
        {

        }

        public XmlMsgNode CreateParamNode(string param, bool reuseIfExist = false)
        {
            if (reuseIfExist && Node[param] != null)
            {
                return new XmlMsgNode(Doc, Node[param]);
            }

            var node = Doc.CreateElement(param);
            Node.AppendChild(node);

            return new XmlMsgNode(Doc, node);
        }

        public void SetParameter(string param, string value)
        {
            XmlNode node = Node[param];
            if (node == null)
            {
                node = Doc.CreateElement(param);
                Node.AppendChild(node);
            }
            else
            {
                node.RemoveAll();
            }

            node.AppendChild(Doc.CreateCDataSection(value));
        }

        public void SetParameter(string param, long value)
        {
            XmlNode node = Node[param];
            if (node == null)
            {
                node = Doc.CreateElement(param);
                Node.AppendChild(node);
            }
            else
            {
                node.RemoveAll();
            }

            node.InnerText = value.ToString();
        }

        public override string ToString()
        {
            return Node.OuterXml;
        }
    }

    public class XmlRequest : XmlMsgNode
    {
        private XmlRequest(XmlDocument doc, XmlNode root)
            : base(doc, root)
        {
        }

        public static XmlRequest From(string xml)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode root = null;

            try
            {
                doc.LoadXml(xml);
                root = doc.DocumentElement;
            }
            catch (Exception)
            {
                return null;
            }

            if (doc == null || root == null)
            {
                return null;
            }

            return new XmlRequest(doc, root);
        }

        public XmlRequest()
        {
            Doc = new XmlDocument();

            Doc.LoadXml("<xml></xml>");

            Node = Doc.DocumentElement;
        }
    }

    public class XmlMessage : XmlMsgNode
    {
        private XmlNode Root;

        public string AgentId { get; set; }
        public string FromUserId { get; set; }

        private XmlMessage(XmlDocument doc, XmlNode root)
            : base(doc, root)
        {
            Root = root;

            AgentId = GetParameter("AgentID");
            FromUserId = GetParameter("FromUserName");
        }

        public static XmlMessage From(string xml)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode root = null;

            try
            {
                doc.LoadXml(xml);
                root = doc.DocumentElement;
            }
            catch (Exception)
            {
                return null;
            }

            if (doc == null || root == null)
            {
                return null;
            }

            string agentId = GetNodeChildInnerText(root, "AgentID");
            if (agentId == null || agentId.Length == 0)
            {
                return null;
            }

            string fromUserId = GetNodeChildInnerText(root, "FromUserName");
            if (fromUserId == null || fromUserId.Length == 0)
            {
                return null;
            }

            return new XmlMessage(doc, root);
        }

        //
        // Update
        //

        private XmlMessage()
        {
            Doc = new XmlDocument();

            Doc.LoadXml("<xml></xml>");

            Node = Doc.DocumentElement;
            Root = Node;
        }

        public static XmlMessage CreateTextResponse(XmlMessage req, string response)
        {
            XmlMessage msg = new XmlMessage();
            msg.SetParameter("ToUserName", req.GetParameter("FromUserName"));
            msg.SetParameter("FromUserName", req.GetParameter("ToUserName"));
            msg.SetParameter("CreateTime", long.Parse(req.GetParameter("CreateTime")));
            msg.SetParameter("MsgType", "text");
            msg.SetParameter("Content", response);

            return msg;
        }

        public static XmlMessage CreateRichResponse(XmlMessage req, IEnumerable<RichMsg> responses)
        {
            XmlMessage msg = new XmlMessage();
            msg.SetParameter("ToUserName", req.GetParameter("FromUserName"));
            msg.SetParameter("FromUserName", req.GetParameter("ToUserName"));
            msg.SetParameter("CreateTime", long.Parse(req.GetParameter("CreateTime")));
            msg.SetParameter("MsgType", "news");
            msg.SetParameter("ArticleCount", responses.Count());

            var articles = msg.CreateParamNode("Articles", true);

            foreach (var response in responses)
            {
                var item = articles.CreateParamNode("item");

                item.SetParameter("Title", response.Title);
                item.SetParameter("Description", response.Description);
                item.SetParameter("PicUrl", response.PicUrl);
                if (!string.IsNullOrEmpty(response.Url))
                {
                    item.SetParameter("Url", response.Url);
                }
            }

            return msg;
        }
        public static XmlMessage CreateRichResponse(XmlMessage req, RichMsg response)
        {
            return CreateRichResponse(req, new RichMsg[] { response });
        }

        public override string ToString()
        {
            return Root.OuterXml;
        }
    }
    */
}