using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polufabrikkat.Core.Models
{
    public class UnsplashApiResponse
    {
        public string Id { get; set; }
        public string Slug { get; set; }
        public AlternativeSlugs AlternativeSlugs { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset? PromotedAt { get; set; }
        public long Width { get; set; }
        public long Height { get; set; }
        public string Color { get; set; }
        public string BlurHash { get; set; }
        public string Description { get; set; }
        public string AltDescription { get; set; }
        public List<object> Breadcrumbs { get; set; }
        public Urls Urls { get; set; }
        public Links Links { get; set; }
        public long Likes { get; set; }
        public bool LikedByUser { get; set; }
        public List<object> CurrentUserCollections { get; set; }
        public object Sponsorship { get; set; }
        public TopicSubmissions TopicSubmissions { get; set; }
        public string AssetType { get; set; }
        public User User { get; set; }
        public Exif Exif { get; set; }
        public Location Location { get; set; }
        public Meta Meta { get; set; }
        public bool PublicDomain { get; set; }
        public List<Tag> Tags { get; set; }
        public long Views { get; set; }
        public long Downloads { get; set; }
        public List<object> Topics { get; set; }
    }

    public class AlternativeSlugs
    {
        public string En { get; set; }
        public string Es { get; set; }
        public string Ja { get; set; }
        public string Fr { get; set; }
        public string It { get; set; }
        public string Ko { get; set; }
        public string De { get; set; }
        public string Pt { get; set; }
    }

    public class Urls
    {
        public string Raw { get; set; }
        public string Full { get; set; }
        public string Regular { get; set; }
        public string Small { get; set; }
        public string Thumb { get; set; }
        public string SmallS3 { get; set; }
    }

    public class Links
    {
        public string Self { get; set; }
        public string Html { get; set; }
        public string Download { get; set; }
        public string DownloadLocation { get; set; }
    }

    public class Exif
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public string ExposureTime { get; set; }
        public string Aperture { get; set; }
        public string FocalLength { get; set; }
        public long Iso { get; set; }
    }

    public class Position
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Location
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Position Position { get; set; }
    }

    public class Meta
    {
        public bool Index { get; set; }
    }

    public class Tag
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public Source Source { get; set; }
    }

    public class Source
    {
        public Ancestry Ancestry { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public CoverPhoto CoverPhoto { get; set; }
    }

    public class Ancestry
    {
        public AncestryType Type { get; set; }
        public AncestryCategory Category { get; set; }
        public AncestrySubcategory Subcategory { get; set; }
    }

    public class AncestryType
    {
        public string Slug { get; set; }
        public string PrettySlug { get; set; }
        public object Redirect { get; set; }
    }

    public class AncestryCategory
    {
        public string Slug { get; set; }
        public string PrettySlug { get; set; }
        public object Redirect { get; set; }
    }

    public class AncestrySubcategory
    {
        public string Slug { get; set; }
        public string PrettySlug { get; set; }
        public object Redirect { get; set; }
    }

    public class CoverPhoto
    {
        public string Id { get; set; }
        public string Slug { get; set; }
        public AlternativeSlugs AlternativeSlugs { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset? PromotedAt { get; set; }
        public long Width { get; set; }
        public long Height { get; set; }
        public string Color { get; set; }
        public string BlurHash { get; set; }
        public string Description { get; set; }
        public string AltDescription { get; set; }
        public List<object> Breadcrumbs { get; set; }
        public Urls Urls { get; set; }
        public Links Links { get; set; }
        public long Likes { get; set; }
        public bool LikedByUser { get; set; }
        public List<object> CurrentUserCollections { get; set; }
        public object Sponsorship { get; set; }
        public TopicSubmissions TopicSubmissions { get; set; }
        public string AssetType { get; set; }
        public bool Premium { get; set; }
        public bool Plus { get; set; }
        public User User { get; set; }
    }

    public class TopicSubmissions
    {
        // Заполните это класс при необходимости
    }

    public class User
    {
        public string Id { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TwitterUsername { get; set; }
        public string PortfolioUrl { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public UserLinks Links { get; set; }
        public ProfileImage ProfileImage { get; set; }
        public string InstagramUsername { get; set; }
        public long TotalCollections { get; set; }
        public long TotalLikes { get; set; }
        public long TotalPhotos { get; set; }
        public long TotalPromotedPhotos { get; set; }
        public long TotalIllustrations { get; set; }
        public long TotalPromotedIllustrations { get; set; }
        public bool AcceptedTos { get; set; }
        public bool ForHire { get; set; }
        public Social Social { get; set; }
    }

    public class UserLinks
    {
        public string Self { get; set; }
        public string Html { get; set; }
        public string Photos { get; set; }
        public string Likes { get; set; }
        public string Portfolio { get; set; }
        public string Following { get; set; }
        public string Followers { get; set; }
    }

    public class ProfileImage
    {
        public string Small { get; set; }
        public string Medium { get; set; }
        public string Large { get; set; }
    }

    public class Social
    {
        public string InstagramUsername { get; set; }
        public string PortfolioUrl { get; set; }
        public string TwitterUsername { get; set; }
        public object PaypalEmail { get; set; }
    }
}
