﻿using SmarterBalanced.SampleItems.Dal.Models;
using SmarterBalanced.SampleItems.Dal.Infrastructure;
using SmarterBalanced.SampleItems.Dal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmarterBalanced.SampleItems.Core.Interfaces;

namespace SmarterBalanced.SampleItems.Core.Infrastructure
{
    public class SampleItemsSearchRepo : ISampleItemsSearchRepo
     {
        private ISampleItemsRepo sampleItemsRepo;
        public SampleItemsSearchRepo(ISampleItemsRepo repo)
        {
            sampleItemsRepo = repo;
        }
    }
}