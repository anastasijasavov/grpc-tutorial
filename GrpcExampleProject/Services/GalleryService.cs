using Grpc.Core;
using GrpcTestProject.Data;
using GrpcTestProject.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcTestProject.Services;

public class GalleryService : Gallery.GalleryBase
{
    private readonly AppDbContext _context;
    public GalleryService(AppDbContext context)
    {
        _context = context;
    }

    public override async Task<CreateGalleryResponse> CreateGallery
    (CreateGalleryRequest request, ServerCallContext context)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Ime galerije je obavezno polje."));
        }

        var gallery = new Models.Gallery
        {
            Name = request.Name,
            Description = request.Description
        };

        await _context.AddAsync(gallery);
        await _context.SaveChangesAsync();

        return await Task.FromResult(
            new CreateGalleryResponse { Id = gallery.Id }
        );
    }
    public override async Task<ReadGalleryResponse> ReadGallery
  (ReadGalleryRequest request, ServerCallContext context)
    {

        if (request.Id <= 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Data galerija ne postoji."));

        }
        var gallery = _context.Galleries.FirstOrDefault(x => x.Id == request.Id);
        if (gallery != null)
        {
            return await Task.FromResult(new ReadGalleryResponse
            {
                Name = gallery.Name,
                Description = gallery.Description,
                Id = gallery.Id
            });
        }
        throw new RpcException(new Status(StatusCode.NotFound, $"Ne postoji data galerija sa id-jem {request.Id}."));
    }

    public override async Task<GetAllResponse> ReadGalleries
 (GetAllRequest request, ServerCallContext context)
    {

        // if (request.Id <= 0)
        // {
        //     throw new RpcException(new Status(StatusCode.InvalidArgument, "Data galerija ne postoji."));

        // }
        var response = new GetAllResponse();
        var galleries = await _context.Galleries.ToListAsync();

        foreach (var item in galleries)
        {
            response.Galleries.Add(new ReadGalleryResponse
            {
                Name = item.Name,
                Description = item.Description,
                Id = item.Id
            });
        }
        return await Task.FromResult(response);
    }

    public override async Task<UpdateGalleryResponse> UpdateGallery
        (UpdateGalleryRequest request, ServerCallContext context)
    {

        if (request.Id == 0 || string.IsNullOrEmpty(request.Name))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Argumenti su invalidni."));
        }
        var gallery = await _context.Galleries.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (gallery == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Gallery with id {request.Id} is not found."));
        }
        gallery.Name = request.Name;
        gallery.Description = request.Description;
        await _context.SaveChangesAsync();

        return await Task.FromResult(new UpdateGalleryResponse
        {
            Id = gallery.Id
        });
    }

    public override async Task<DeleteGalleryResponse> DeleteGallery
        (DeleteGalleryRequest request, ServerCallContext context)
    {
        if (request.Id <= 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, $"Resurs s id-jem {request.Id} nije pronadjen."));
        }
        var gallery = await _context.Galleries.FirstOrDefaultAsync(x => x.Id == request.Id);

        if (gallery == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Gallery with id {request.Id} is not found."));
        }
        _context.Galleries.Remove(gallery);
        await _context.SaveChangesAsync();

        return await Task.FromResult(new DeleteGalleryResponse
        {
            Id = gallery.Id
        });

    }

    public override async Task<MultiGalleryResponse> UpdateGalleriesPhotos
    (IAsyncStreamReader<AddGalleryPhoto> requestStream,
     ServerCallContext context)
    {

        var response = new MultiGalleryResponse
        {
            GalleryResponse = { }
        };
        await foreach (var request in requestStream.ReadAllAsync())
        {
            var image = new Models.Photo
            {
                ImagePath = request.ImagePath,
                Year = request.Year,
                Name = request.Name,
                GalleryId = request.GalleryId

            };
            _context.Photos.Add(image);
            await _context.SaveChangesAsync();
            response.GalleryResponse.Add(new UpdateGalleryResponse { Id = image.Id });
        }
        return response;
    }
}