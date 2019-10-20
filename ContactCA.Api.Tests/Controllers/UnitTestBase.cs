﻿using AppCore;
using Autofac;
using ContactCA.Api.Controllers;
using ContactCA.Data;
using ContactCA.Data.Entities;
using ContactCA.Data.Repositories.Contracts;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;

namespace ContactCA.Api.Tests.Controllers
{
   public class UnitTestBase
   {
      private IContainer _container;
      protected IContainer Container
      {
         get
         {
            if (_container == null)
            {
               var builder = new ContainerBuilder();

               builder.RegisterType<ComponentResolver>().As<IComponentResolver>().SingleInstance();

               // db context .. if needed
               builder.RegisterType<ContactCADbContext>().As<ContactCADbContext>().InstancePerLifetimeScope();

               // Repositories
               builder.RegisterInstance(GetMockContactRepository());

               // Register the CompanyDataRepository for property injection not constructor allowing circular references
               //builder.RegisterType<ContactRepository>().As<IContactRepository>()
               //       .InstancePerLifetimeScope()
               //       .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

               // register controllers
               builder.RegisterType<ContactController>().As<IHttpController>().InstancePerLifetimeScope();

               var container = builder.Build();

               _container = container;
            }

            return _container;
         }
      }

      protected ContactCADbContext ContactCADbContext
      {
         get
         {
            return Container.Resolve<ContactCADbContext>();
         }
      }

      protected IContactRepository GetMockContactRepository()
      {
         // mock one up ...

         var mockContactRepository = new Mock<IContactRepository>();

         List<Contact> contacts = new List<Contact>()
         {
            new Contact()
            {
               ContactID = 1,
               FirstName = "Mike",
               LastName = "Tyson",
               Telephone = "2223334444",
               BestCallTime = DateTime.UtcNow.AddDays(1).AddHours(1).AddMinutes(15),
               DateCreated = DateTime.UtcNow.AddDays(-3).AddHours(1).AddMinutes(15)
            },
            new Contact()
            {
               ContactID = 2,
               FirstName = "Yogi",
               LastName = "Bear",
               Telephone = "2223334444",
               BestCallTime = DateTime.UtcNow.AddDays(1).AddHours(3).AddMinutes(30),
               DateCreated = DateTime.UtcNow.AddDays(-2).AddHours(3).AddMinutes(30)
            },
            new Contact()
            {
               ContactID = 3,
               FirstName = "Donald",
               LastName = "Trump",
               Telephone = "2223334444",
               BestCallTime = DateTime.UtcNow.AddDays(1).AddHours(1).AddMinutes(15),
               DateCreated = DateTime.UtcNow.AddDays(-1).AddHours(5).AddMinutes(45)
            }
         };

         mockContactRepository.Setup(x => x.GetContacts()).Returns(contacts);

         mockContactRepository.Setup(x => x.GetContactsByFirstName(It.IsAny<string>()))
            .Returns((string firstName) => contacts.Where(x => x.FirstName == firstName));

         mockContactRepository.Setup(x => x.GetContactById(It.IsAny<int>()))
            .Returns((int id) => contacts.FirstOrDefault(x => x.ContactID == id));

         return mockContactRepository.Object;
      }
   }
}