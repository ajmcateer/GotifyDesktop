using gotifySharp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Models
{
    public class ExtendedApplicationModel : ApplicationModel
    {
        public bool HasAlert {get; set;}

        public ExtendedApplicationModel(ApplicationModel applicationModel)
        {
            this.description = applicationModel.description;
            this.HasAlert = false;
            this.id = applicationModel.id;
            this.image = applicationModel.image;
            this.name = applicationModel.name;
            this.token = applicationModel.token;
            this._internal = applicationModel._internal;
        }
    }
}
