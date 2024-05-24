using Pharmacy_.Interfaces;

namespace Pharmacy_.Repositories;

public class PhotoService : IPhotoService
    {
        private readonly ILogger<PhotoService> _logger;
        private readonly string _rootLocation = "uploads"; // директория для сохранения фотографий

        public PhotoService(ILogger<PhotoService> logger)
        {
            _logger = logger;
        }

        public async Task SavePhotoAsync(Guid userId, Stream fileStream, int role)
        {
            try
            {
                string location = role == 2 ? $"item/{userId}" : $"manufacturer/{userId}";
                string userDir = Path.Combine(_rootLocation, location);
                Directory.CreateDirectory(userDir);

                string filePath = Path.Combine(userDir, $"{userId}.jpg");

                // Удаляем файл, если он уже существует
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Сохраняем новый файл
                using (var file = File.Create(filePath))
                {
                    await fileStream.CopyToAsync(file);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось сохранить фото для пользователя {userId}: {ex.Message}");
                throw;
            }
        }

        public async Task<Stream> GetPhotoAsync(Guid userId, int role)
        {
            try
            {
                string location = role == 2 ? $"item/{userId}" : $"manufacturer/{userId}";
                string filePath = Path.Combine(_rootLocation, location, $"{userId}.jpg");

                if (!File.Exists(filePath))
                {
                    // возвращаем стандартное фото, если фото пользователя не существует
                    filePath = Path.Combine(_rootLocation, "profileIcon.jpg");
                }

                // Читаем файл и возвращаем как поток
                return await Task.FromResult(File.OpenRead(filePath));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Не удалось получить фото для пользователя {userId}: {ex.Message}");
                throw;
            }
        }
    }