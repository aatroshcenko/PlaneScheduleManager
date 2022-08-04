using Google.Protobuf;

namespace PlaneScheduleManager.Server.Services.Interfaces
{
    public interface IAudioGenerator
    {
        public Task<ByteString> SynthesizeSpeechAsync(string text);
    }
}
