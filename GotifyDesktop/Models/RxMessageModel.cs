using gotifySharp.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GotifyDesktop.Models
{
    public class RxMessageModel : ReactiveObject
    {
        public int _id;
        public int _appid;
        public string _message;
        public string _dateString;
        public string _title;
        public int _priority;
        public DateTime _date;

        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public int Appid
        {
            get => _appid;
            set => this.RaiseAndSetIfChanged(ref _appid, value);
        }
        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public int Priority
        {
            get => _priority;
            set => this.RaiseAndSetIfChanged(ref _priority, value);
        }

        public DateTime Date
        {
            get => _date;
            set => this.RaiseAndSetIfChanged(ref _date, value);
        }

        public string DateString
        {
            get => _dateString;
            set => this.RaiseAndSetIfChanged(ref _dateString, value);
        }

        public RxMessageModel(MessageModel messageModel)
        {
            Id = messageModel.Id;
            Appid = messageModel.Appid;
            Message = messageModel.Message;
            Title = messageModel.Title;
            Date = messageModel.Date;
            Priority = messageModel.Priority;
            DateString = messageModel.Date.ToString("MM/dd/yyyy HH:mm:ss");
        }

        public RxMessageModel()
        {

        }
    }
}
