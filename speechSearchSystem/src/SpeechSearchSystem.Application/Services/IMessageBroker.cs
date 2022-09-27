using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechSearchSystem.Application.Services;

public interface IMessageBroker
{
    void SendMessage<T> (T message);
}