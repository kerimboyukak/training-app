using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Training.Mobile.Models;

namespace Training.Mobile.Messages
{
    public class TrainingSelectedMessage : ValueChangedMessage<TrainingDetail>
    {
        public TrainingSelectedMessage(TrainingDetail value) : base(value)
        {
        }
    }
}
