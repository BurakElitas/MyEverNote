using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    public class MyInitializer :CreateDatabaseIfNotExists<DatabaseContext>
    {
        protected override void Seed(DatabaseContext context)
        {
            //Adding Admin User
            EvernoteUser admin = new EvernoteUser()
            {
                Name = "Burak",
                Surname = "elitaş",
                Email = "burak.elitas@outlook.com.tr",
                ActivateGuid = Guid.NewGuid(),
                IsAdmin = true,
                IsActive = true,
                Username = "burakelitas",
                ProfileImageFileName="user_boy.png",
                Password = "12345",
                CreatedOn = DateTime.Now.AddHours(1),
                ModifiedOn = DateTime.Now.AddMinutes(65),
                ModidiedUsername = "burakelitas"

            };

            //Adding standart User
            EvernoteUser standartUser = new EvernoteUser()
            {
                Name = "muhammet",
                Surname = "elitaş",
                Email = "muhammet.elitas@outlook.com.tr",
                ActivateGuid = Guid.NewGuid(),
                IsAdmin = false,
                IsActive = true,
                Username = "muhammetelitas",
                ProfileImageFileName = "user_boy.png",
                Password = "12345",
                CreatedOn = DateTime.Now.AddHours(2),
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModidiedUsername = "muhammetelitas"

            };
            context.EvernoteUsers.Add(admin);
            context.EvernoteUsers.Add(standartUser);


            //Adding Fake User
            for (int i = 0; i < 8; i++)
            {
                EvernoteUser user = new EvernoteUser()
                {
                    Name = FakeData.NameData.GetFirstName(),
                    Surname = FakeData.NameData.GetSurname(),
                    Email = FakeData.NetworkData.GetEmail(),
                    ActivateGuid = Guid.NewGuid(),
                    IsAdmin = false,
                    ProfileImageFileName = "user_boy.png",
                    IsActive = true,
                    Username = $"user{i}",
                    Password = "123",
                    CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModidiedUsername = $"user{i}"
                };
                context.EvernoteUsers.Add(user);

            }
            context.SaveChanges();

            //User List for using..
            List<EvernoteUser> users = context.EvernoteUsers.ToList();

            //Adding fake Categories

            for (int i = 0; i < 10; i++)
            {
                Category cat = new Category()
                {
                    Title = FakeData.PlaceData.GetStreetName(),
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    Description = FakeData.PlaceData.GetAddress(),
                    ModidiedUsername = "burakelitas"
                };

                context.Categories.Add(cat);


                //Adding fake Notes..

                for (int k = 0; k < FakeData.NumberData.GetNumber(5,9); k++)
                {
                    EvernoteUser owner = users[FakeData.NumberData.GetNumber(0, users.Count-1)];

                    Note note = new Note()
                    {
                       
                        IsDraft = false,
                        Text = FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(1, 3)),
                        Title = FakeData.TextData.GetAlphabetical(FakeData.NumberData.GetNumber(5, 25)),
                        LikeCount = FakeData.NumberData.GetNumber(1, 9),
                        Owner = owner,
                        CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModidiedUsername = owner.Username,
                    };

                    cat.Notes.Add(note);

                   
                    //Adding fake Comments
                    for (int j = 0; j < FakeData.NumberData.GetNumber(3,5); j++)
                    {
                        owner = users[FakeData.NumberData.GetNumber(0, users.Count - 1)];
                        Comment comment = new Comment()
                        {
                            Text = FakeData.TextData.GetSentence(),
                            Owner = owner,
                            CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModidiedUsername = owner.Username

                        };

                        note.Comments.Add(comment);
                    }

                    //Adding Fake Likes

                    for (int m = 0; m < note.LikeCount; m++)
                    {
                        Liked liked = new Liked()
                        {
                            LikedUser = users[m]
                        };
                        note.Likes.Add(liked);

                    }



                }



            }
            context.SaveChanges();

        }
    }
}
