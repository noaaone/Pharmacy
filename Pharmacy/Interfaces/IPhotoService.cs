namespace Pharmacy_.Interfaces;

public interface IPhotoService
{
    Task SavePhotoAsync(int userId, Stream fileStream, int role);
    Task<Stream> GetPhotoAsync(int userId, int role);
}