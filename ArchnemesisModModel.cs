using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poe_archnemesis_acs
{
    class ArchnemesisModModel
    {
        private string name, imgSrc;
        private string count;

        private List<string> archnemesisMod;
        private List<List<string>> archnemesisMods;

        public List<List<string>> ArchnemesisMods
        {
            get { return archnemesisMods; }
            set { archnemesisMods = value; }
        }

        public List<string> ArchnemesisMod
        {
            get { return archnemesisMod; }
            set { archnemesisMod = value; }
        }

        private string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string ImageSource
        {
            get { return imgSrc; }
            set { imgSrc = value; }
        }

        private string Count
        {
            get { return count; }
            set { count = value; }
        }


    }
}
