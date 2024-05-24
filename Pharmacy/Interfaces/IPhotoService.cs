namespace Pharmacy_.Interfaces;

public interface IPhotoService
{
    Task SavePhotoAsync(Guid userId, Stream fileStream, int role);
    Task<Stream> GetPhotoAsync(Guid userId, int role);
}