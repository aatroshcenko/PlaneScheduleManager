using Google.Cloud.TextToSpeech.V1;
using Google.Protobuf;
using PlaneScheduleManager.Server.Services.Interfaces;

namespace PlaneScheduleManager.Server.Services
{
    public class AudioGenerator: IAudioGenerator
    {
        private readonly TextToSpeechClient _textToSpeechClient;
        public AudioGenerator(
            TextToSpeechClient textToSpeechClient)
        {
            _textToSpeechClient = textToSpeechClient;
        }

        public async Task<ByteString> SynthesizeSpeechAsync(string text)
        {
            var input = new SynthesisInput
            {
                Text = text
            };
            var voiceSelection = new VoiceSelectionParams
            {
                LanguageCode = "en-US",
                SsmlGender = SsmlVoiceGender.Female
            };
            var audioConfig = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3
            };
            SynthesizeSpeechResponse response = await _textToSpeechClient
                .SynthesizeSpeechAsync(input, voiceSelection, audioConfig);

            return response.AudioContent;
        }
    }
}
