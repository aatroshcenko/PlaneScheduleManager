using IoTDevice.Client.Services.Interfaces;
using NetCoreAudio.Interfaces;

namespace IoTDevice.Client.Services
{
    public class AudioManager: IAudioManager
    {
        private readonly IPlayer _player;

        public AudioManager(
            IPlayer player)
        {
            _player = player;
        }
        public async Task<string> CreateTempMp3FileAsync(string audioBase64)
        {
            string fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".mp3";
            await File.WriteAllBytesAsync(fileName, Convert.FromBase64String(audioBase64));
            return fileName;
        }

        public async Task PlayAudioAsync(string fileName)
        {
            await _player.Play(fileName);
        }
    }
}
