using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BakedRaspberryPi
{
    public class EmailMessageMaker
    {
        public string EmailLinkHref { get; set; }
        public string ImagesPath { get; set; }
        public string EmailHeaderImage { get; set; }
        public string EmailFooterImage { get; set; }
        public IList<string> Line { get; private set; }

        public EmailMessageMaker()
        {
            this.Line = new List<string>();
        }

        public EmailMessageMaker(string headerImage, string footerImage, string imagePath, string linkHref)
        {
            EmailLinkHref = linkHref;
            ImagesPath = imagePath;
            EmailHeaderImage = headerImage;
            EmailFooterImage = footerImage;

            this.Line = new List<string>();
        }

        public string GetTheString()
        {
            string stringToReturn = "";
            stringToReturn += GetTheHeader();
            foreach (string line in Line)
            {
                stringToReturn += line;
                stringToReturn += "<br />";
            }
            stringToReturn += GetTheFooter();
            return stringToReturn;
        }

        public string GetTheHeader()
        {
            string theStringToReturn;
            if (EmailHeaderImage == null)
            {
                theStringToReturn = "<html><body><br /><img src='http://www.sot-bakery.com/images/emailHeader.jpg' />";
            }
            else
            {
                theStringToReturn = "<html><body><br /><a href=\"" + GetTheLink() + "\"><img src=\"cid:" + EmailHeaderImage + "\"></a><br /><br />";
            }
            return theStringToReturn;
        }

        public string GetTheFooter()
        {
            string theStringToReturn;
            if (EmailHeaderImage == null)
            {
                theStringToReturn = "<img src='http://www.sot-bakery.com/images/emailFooter.jpg' /></body></html>";
            }
            else
            {
                theStringToReturn = "<a href=\"" + GetTheLink() + "><img src=\"cid:"+ EmailFooterImage + "\"></a></body></html>";
            }
            return theStringToReturn;
        }

        public string GetTheLink()
        {
            string theStringToReturn;
            if (EmailLinkHref == "")
            {
                theStringToReturn = "#";
            }
            else
            {
                theStringToReturn = EmailLinkHref;
            }
            return theStringToReturn;
        }
    }

}