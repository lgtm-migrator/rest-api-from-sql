﻿using System;
using System.Collections.Generic;

namespace CodeGenerator.Projects
{
    public class ProjectFile
    {
        public Guid Id { get; internal set; }
        public string Name { get; }
        internal Guid SectionId { get; }
        internal string FileName => $"{Name}.csproj";

        public IList<Class> ClassFiles { get; } = new List<Class>();

        public ProjectFile(string name)
        {
            Id = Guid.Empty;
            Name = name;
            SectionId = Guid.NewGuid();
        }
    }
}