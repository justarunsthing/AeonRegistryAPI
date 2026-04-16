using AeonRegistryAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AeonRegistryAPI.Services
{
    public class ArtifactMediaFileService(ApplicationDbContext db) : IArtifactMediaFileService
    {
        public async Task<ArtifactMediaFile?> CreateArtifactMediaFileAsync(int artifactId, IFormFile file, bool isPrimary, CancellationToken ct)
        {
            var artifact = await db.Artifacts.FindAsync([artifactId], ct);

            if (artifact is null)
            {
                return null;
            }

            if (file is null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be empty!");
            }

            if (isPrimary)
            {
                var existingPrimary = await db.ArtifactMediaFiles
                    .Where(m => m.ArtifactId == artifactId && m.IsPrimary)
                    .ToListAsync(ct);

                foreach (var media in existingPrimary)
                {
                    media.IsPrimary = false;
                }
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            var data = ms.ToArray();

            var mediaFile = new ArtifactMediaFile
            {
                ArtifactId = artifactId,
                FileName = file.FileName,
                ContentType = file.ContentType,
                Data = data,
                IsPrimary = isPrimary
            };

            db.ArtifactMediaFiles.Add(mediaFile);
            await db.SaveChangesAsync(ct);

            return mediaFile;
        }
    }
}