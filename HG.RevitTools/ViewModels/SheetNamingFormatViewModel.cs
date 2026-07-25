using HG.RevitTools.Models.SheetCreator;
using System.Collections.Generic;

namespace HG.RevitTools.ViewModels
{
    public class SheetNamingFormatViewModel
    {
        public string Template { get; set; } =
            "{number} - {name}";

        public IList<SheetNamingToken> AvailableTokens { get; }

        public SheetNamingFormatViewModel()
        {
            AvailableTokens =
                new List<SheetNamingToken>
                {
                    new SheetNamingToken
                    {
                        Token = "{number}",
                        Description = "Full generated sheet number",
                        Example = "BIA1-SPK-B01-TD-L12-DW-EL-ZZ-12101-00"
                    },

                    new SheetNamingToken
                    {
                        Token = "{name}",
                        Description = "View name / drawing title",
                        Example = "B02-02-Level2-FFL - J12.1"
                    },

                    new SheetNamingToken
                    {
                        Token = "{project}",
                        Description = "Project number",
                        Example = "BIA1"
                    },

                    new SheetNamingToken
                    {
                        Token = "{originator}",
                        Description = "Originator code",
                        Example = "SPK"
                    },

                    new SheetNamingToken
                    {
                        Token = "{system}",
                        Description = "System / facility code",
                        Example = "B01"
                    },

                    new SheetNamingToken
                    {
                        Token = "{phase}",
                        Description = "Project phase",
                        Example = "TD"
                    },

                    new SheetNamingToken
                    {
                        Token = "{location}",
                        Description = "Location code from scope box",
                        Example = "L12"
                    },

                    new SheetNamingToken
                    {
                        Token = "{type}",
                        Description = "Drawing type",
                        Example = "DW"
                    },

                    new SheetNamingToken
                    {
                        Token = "{role}",
                        Description = "Role / discipline",
                        Example = "EL"
                    },

                    new SheetNamingToken
                    {
                        Token = "{level}",
                        Description = "Level code",
                        Example = "ZZ"
                    },

                    new SheetNamingToken
                    {
                        Token = "{package}",
                        Description = "Package code",
                        Example = "1"
                    },

                    new SheetNamingToken
                    {
                        Token = "{scale}",
                        Description = "Scale code",
                        Example = "2"
                    },

                    new SheetNamingToken
                    {
                        Token = "{serial}",
                        Description = "Serial number",
                        Example = "101"
                    },

                    new SheetNamingToken
                    {
                        Token = "{revision}",
                        Description = "Revision number",
                        Example = "00"
                    }
                };
        }
    }
}