using BakedRaspberryPi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BakedRaspberryPi
{
    public class EditButtonHelperService : Controller
    {
        /// <summary>
        /// This function returns an HTML string for either an Edit Button or an Add Button for components of a WholePi.
        /// The button will automatically call the "Edit" Action of the specified Controller.
        /// </summary>
        public string MakeACartEditButton(string buttonType, string controller, WholePi thePi, int theId, int? previous, string buttonText)
        {
            string stringToReturn;
            string glyph = "edit";

            if (previous == null)
            {
                previous = 1;
                glyph = "plus";
            }
            stringToReturn =
                String.Format(
                    "<a class=\"btn btn-" +
                    "{0}\" href=\"/" +
                    "{1}/Edit?wholePiPi=" +
                    "{2}&wholePiToBeEditedId=" +
                    "{3}&previous=" +
                    "{4}\"><span class=\"glyphicon glyphicon-" +
                    "{5}\" aria-hidden=\"true\"></span>" +
                    "{6}</a>",
                    buttonType,
                    controller,
                    thePi.ToString(),
                    theId,
                    previous,
                    glyph,
                    buttonText
                    );

            return stringToReturn;
        }
        
        /// <summary>
        /// This function returns an HTML string for either an Edit Button or an Add Button for components of a WholePi.
        /// It allows for the use of a specific Action within the Controller that is called.
        /// </summary>
        public string MakeACartEditButton(string buttonType, string controller, string action, WholePi thePi, int theId, string previous, string buttonText)
        {
            string stringToReturn;
            string glyph = "edit";
            
            if (previous == null)
            {
                glyph = "plus";
            }
            stringToReturn =
                String.Format(
                    "<a class=\"btn btn-" +
                    "{0}\" href=\"/" +
                    "{1}/" +
                    "{2}?wholePiPi=" +
                    "{3}&wholePiToBeEditedId=" +
                    "{4}&previousString=" +
                    "{5}\"><span class=\"glyphicon glyphicon-" +
                    "{6}\" aria-hidden=\"true\"></span>" +
                    "{7}</a>",
                    buttonType,
                    controller,
                    action,
                    thePi.ToString(),
                    theId,
                    previous,
                    glyph,
                    buttonText
                    );

            return stringToReturn;
        }
    }
}