﻿namespace BlingMeMVC.Models.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    using BlingMe.Domain.Entities;

    public class BraceletView
    {
        public BraceletView(Bracelet bracelet, Bracelet loggedOnUserBracelet)
        {
            ID = bracelet.ID;
            Name = bracelet.Name;
            Description = bracelet.Description;
            Owner = bracelet.Owner;
            Type = bracelet.Type;
            Email = bracelet.Email;
            Avatar = bracelet.Avatar;
            if (Avatar == null)
            {
                ImageUrl = "../Content/Images/Charm_"
                           + bracelet.Type.ToString().ToLower() + ".png";
            }
            else
            {
                ImageUrl = Utilities.GetImageUrlString(ID);
            }
            LoggedOnUserBracelet = loggedOnUserBracelet;

            Children = new List<Bracelet>();
            Parents = new List<Bracelet>();
            StrayChildren = new List<Bracelet>();
            StrayParents = new List<Bracelet>();

            if (null != bracelet.Charms)
            {
                Children = (from c in bracelet.Charms
                            from child in c.Children
                            where c.Parent.ID == bracelet.ID
                            && child.Type == Type
                            select child).ToList();

                Parents = (from c in bracelet.Charms
                           where
                               (from child in c.Children
                                where child.ID == bracelet.ID
                                select child).Any() && c.Parent.Type == Type
                           select c.Parent).ToList();

                StrayChildren = (from c in bracelet.Charms
                                 from child in c.Children
                                 where c.Parent.ID == bracelet.ID
                                 && child.Type != Type
                                 select child).ToList();

                StrayParents = (from c in bracelet.Charms
                                 where
                                     (from child in c.Children
                                      where child.ID == bracelet.ID
                                      select child).Any()
                                 && c.Parent.Type != Type
                                 select c.Parent).ToList();

                LoggedOnUserIsChild = bracelet.Charms.Any(c => c.Children
                    .Any(child => child.ID == loggedOnUserBracelet.ID && child.Type == BraceletType.Person));
            }


            DirectReportEmails = "";
            foreach (var c in Children)
            {
                    DirectReportEmails += (c.Email + ";");
            }
            foreach (var c in StrayChildren)
            {
                    DirectReportEmails += (c.Email + ";");
            }
            while (DirectReportEmails.IndexOf(";;") > -1)
            {
                DirectReportEmails = DirectReportEmails.Replace(";;", ";");
            }


            CharmPics = new List<CharmPic>();
            const int PicGap = 30; // degrees

            if (Parents.Count() == 1)
            {
                CharmPics.Add(new CharmPic(Parents.First(), CharmPic.CharmLocation.Parent));
                CharmPics.Last().Degrees = -90;
            } 
            else if (Parents.Count() > 1 && Parents.Count() < 6)
            {
                var parentDrawStart = -90 - (PicGap * (Parents.Count() / 2));
                for (int i = 0; i < Parents.Count(); i++)
                {
                    CharmPics.Add(new CharmPic(Parents[i], CharmPic.CharmLocation.Parent));
                    CharmPics.Last().Degrees = parentDrawStart + (PicGap * i);
                }
            }
            else
            {
                foreach (var parent in Parents)
                {
                    CharmPics.Add(new CharmPic(parent, CharmPic.CharmLocation.Parent));
                    CharmPics.Last().OffBracelet = true;
                }
            }

            if (Children.Count() == 1)
            {
                CharmPics.Add(new CharmPic(Children.First(), CharmPic.CharmLocation.Child));
                CharmPics.Last().Degrees = 90;
            }
            else if (Children.Count() > 1 && Children.Count() < 6)
            {
                var childDrawStart = 90 - (PicGap * (Children.Count() / 2));
                for (int i = 0; i < Children.Count(); i++)
                {
                    CharmPics.Add(new CharmPic(Children[i], CharmPic.CharmLocation.Child));
                    CharmPics.Last().Degrees = childDrawStart + (PicGap * i);
                }
            }
            else
            {
                foreach (var child in Children)
                {
                    CharmPics.Add(new CharmPic(child, CharmPic.CharmLocation.Child));
                    CharmPics.Last().OffBracelet = true;
                }
            }

            var strayParentDrawStart = 180 - (PicGap * (StrayParents.Count() / 2));
            for (int i = 0; i < StrayParents.Count(); i++)
            {
                CharmPics.Add(new CharmPic(StrayParents[i], CharmPic.CharmLocation.StrayParent));
                CharmPics.Last().Degrees = strayParentDrawStart + (PicGap * i);
            }

            var strayChildDrawStart = 0 - (PicGap * (StrayChildren.Count() / 2));
            for (int i = 0; i < StrayChildren.Count(); i++)
            {
                CharmPics.Add(new CharmPic(StrayChildren[i], CharmPic.CharmLocation.StrayChild));
                CharmPics.Last().Degrees = strayChildDrawStart + (PicGap * i);
            }
        }


        public string Description { get; set; }

        public string Owner { get; set; }

        public string Email { get; set; }

        public BraceletType Type { get; set; }

        public int ID { get; set; }

        public string DisplayName { get; set; }

        public string Name { get; set; }

        public byte[] Avatar { get; set; }

        public string DirectReportEmails { get; set; }

        public List<CharmPic> CharmPics { get; set; } 

        public List<Bracelet> Parents { get; set; }
        public List<Bracelet> Children { get; set; }
        public List<Bracelet> StrayParents { get; set; }
        public List<Bracelet> StrayChildren { get; set; }

        public Bracelet LoggedOnUserBracelet { get; set; }
        public bool LoggedOnUserIsChild { get; set; }

        public string ImageUrl { get; set; }
    }
}