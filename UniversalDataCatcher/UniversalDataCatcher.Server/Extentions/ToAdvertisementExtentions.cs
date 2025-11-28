using UniversalDataCatcher.Server.Bots.EvTen.Models;
using UniversalDataCatcher.Server.Entities;

namespace UniversalDataCatcher.Server.Extentions
{
    public static class ToAdvertisementExtentions
    {
        public static Advertisement ToAdvertisement(this EvTenPropertyDetails details)
        {
            var adv = new Advertisement();
            adv.Bina_Id = details.Id;
            adv.Main_Title = details.MainTitle;
            adv.Address = details.Address;
            adv.Area = details.Area.ToString();
            adv.TorpaqArea = details.LandArea.ToString();
            adv.Amount = details.Price.ToString();
            adv.Renovation = details.Renovation;
            adv.Document = details.Document;
            adv.Ipoteka = details.Ipoteka;
            adv.BinaType = details.BinaType;
            adv.Room = details.Rooms.ToString();
            adv.Floor = details.Floor.ToString();
            adv.Category = details.Category;
            adv.Item_Id = details.Id.ToString();
            adv.Poster_Name = details.OwnerName;
            adv.Poster_Note = details.Description;
            adv.Post_Tip = details.PostType;
            adv.Poster_Type = details.PosterType;
            adv.Poster_Phone = details.PhoneNumber;
            adv.Post_Create_Date = details.CreatedAt.ToString();
            adv.InsertDate = DateTime.Now;
            adv.Updated = DateTime.Now;
            adv.Sayt = "Ev10";
            adv.Sayt_Link = details.AdvLink;
            return adv;
        }
    }
}
