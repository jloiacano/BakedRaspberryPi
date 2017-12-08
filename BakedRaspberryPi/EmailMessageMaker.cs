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

        /// <summary>
        /// This is the base constructor for the EmailMessageMaker which makes the message body to be used with <see cref="PiMailer"/>.
        /// </summary>
        public EmailMessageMaker()
        {
            this.Line = new List<string>();
        }

        /// <summary>
        /// This function creates the message body of an Email to be used with <see cref="PiMailer"/>.
        /// The first input is the name of the header image without any path (myHeaderImage.jpg) NULLABLE<para />
        /// The second input is the name of the footer image without any path (myFooterImage.jpg) NULLABLE<para />
        /// The third input is the path to the images (both the header and footer images must be in the same directory) (this is NULLABLE if no images were used)<para />
        /// The fourth input is the link that the header and footer will take the user to if they are clicked. NULLABLE<para />
        /// Once the object is created, use 'OBJECT'.Line.Add("<h1>text in an h1 format</h1>"); to create the message body in order.<para />
        /// Upon completion of adding lines, use <see cref="GetTheString"/> to get the full message body, <see cref="GetTheHeader"/>, <see cref="GetTheFooter"/>, and 
        /// <see cref="GetTheLink"/> to retrieve those properties.
        /// </summary>
        public EmailMessageMaker(string headerImage, string footerImage, string imagePath, string linkHref)
        {
            EmailLinkHref = linkHref;
            ImagesPath = imagePath;
            EmailHeaderImage = headerImage;
            EmailFooterImage = footerImage;

            this.Line = new List<string>();
        }

        /// <summary>
        /// Returns the full body string of an <see cref="EmailMessageMaker"/> after all lines have been added.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the Header image information of an <see cref="EmailMessageMaker"/>.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the Footer image information of an <see cref="EmailMessageMaker"/>.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the Link reference information of an <see cref="EmailMessageMaker"/>.
        /// </summary>
        /// <returns></returns>
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