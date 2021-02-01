using gotifySharp.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Models
{
    public class RxApplicationModel : ReactiveObject
    {
        int _id;
        string _token;
        string _name;
        string _description;
        bool _internal;
        string _image;
        bool _hasAlert;

        public bool HasAlert
        {
            get => _hasAlert;
            set => this.RaiseAndSetIfChanged(ref _hasAlert, value);
        }        
        
        public string Token
        {
            get => _token;
            set => this.RaiseAndSetIfChanged(ref _token, value);
        }        
        
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }        
        
        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }                
        
        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        } 
        
        public string Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }        

        public RxApplicationModel(ApplicationModel applicationModel)
        {
            Description = applicationModel.description;
            HasAlert = false;
            Id = applicationModel.id;
            Image = applicationModel.image;
            Name = applicationModel.name;
            Token = applicationModel.token;
            _internal = applicationModel._internal;
        }
    }
}
