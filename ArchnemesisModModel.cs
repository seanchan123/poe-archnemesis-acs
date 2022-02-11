using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poe_archnemesis_acs
{
    class ArchnemesisModModel
    {
        private string name, imgSrc, ctrlName;
        private int count;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string ImageSource
        {
            get { return imgSrc; }
            set { imgSrc = value; }
        }

        public string ControlName
        {
            get { return ctrlName; }
            set { ctrlName = value; }
        }

        public int Count
        {
            get { return count; }
            set { count = value; }
        }


    }
}
