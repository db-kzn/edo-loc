using EDO_FOMS.Application.Interfaces.Services;
using EDO_FOMS.Domain.Entities.Dir;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Domain.Entities.Public;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Infrastructure.Contexts;
using EDO_FOMS.Infrastructure.Helpers;
using EDO_FOMS.Infrastructure.Models.Identity;
using EDO_FOMS.Shared.Constants.Permission;
using EDO_FOMS.Shared.Constants.Role;
using EDO_FOMS.Shared.Constants.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EDO_FOMS.Infrastructure
{
    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly ILogger<DatabaseSeeder> _logger;
        private readonly IStringLocalizer<DatabaseSeeder> _localizer;
        private readonly EdoFomsContext _db;
        private readonly UserManager<EdoFomsUser> _userManager;
        private readonly RoleManager<EdoFomsRole> _roleManager;

        public DatabaseSeeder(
            UserManager<EdoFomsUser> userManager,
            RoleManager<EdoFomsRole> roleManager,
            EdoFomsContext db, // Database
            ILogger<DatabaseSeeder> logger,
            IStringLocalizer<DatabaseSeeder> localizer
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _logger = logger;
            _localizer = localizer;
        }

        public void Initialize()
        {
            MailGroupParams();
            HomePageParams();

            CreateRoles();
            CreateDocTypes();
            //CreateOrgTypes();

            AddFond();

            //AddSmo1();
            //AddSmo2();
            //AddSmo3();

            //AddMo1();
            //AddMo2();

            //AddUserWoOrg();
        }

        private void MailGroupParams()
        {
            Task.Run(async () =>
            {
                var mail = _db.ParamGroups.FirstOrDefault(c => c.Name == "MailServer");
                if (mail == null)
                {
                    mail = new ParamGroup()
                    {
                        Name = "MailServer",
                        Version = 1,
                        Params = new()
                        {
                            new() { Property = "From", Value = "edo@azino.ru" },
                            new() { Property = "Host", Value = "smtp.yandex.ru" },
                            new() { Property = "Port", Value = "465" },
                            new() { Property = "UserName", Value = "edo@azino.ru" },
                            new() { Property = "Password", Value = "wsrbzrtuhcnzydyf" },
                            new() { Property = "DisplayName", Value = "ЭДО ДЕМОСТЕНД" },
                            new() { Property = "MailPattern", Value = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$" }
                        }
                    };

                    await _db.ParamGroups.AddAsync(mail);
                    await _db.SaveChangesAsync();

                    _logger.LogInformation(_localizer["Seeded Mail Server Group Params"]);
                }
            }).GetAwaiter().GetResult();
        }

        private void HomePageParams()
        {
            Task.Run(async () =>
            {
                var home = _db.ParamGroups.FirstOrDefault(c => c.Name == "HomePage");
                if (home == null)
                {
                    home = new ParamGroup()
                    {
                        Name = "HomePage",
                        Version = 1,
                        Params = new()
                        {
                            new() { Property = "Title", Value = "ЭДО ОМС" },
                            new() { Property = "Description", Value = "Электронный документооборот в сфере ОМС" },
                            new() { Property = "DocSupportPhone", Value = "(XXX) XXX-XX-XX" },
                            new() { Property = "DocSupportEmail", Value = "" },
                            new() { Property = "TechSupportPhone", Value = "(XXX) XXX-XX-XX, (XXX) XXX-XX-XX" },
                            new() { Property = "TechSupportEmail", Value = "edo_support@ffoms.ru" }
                        }
                    };

                    await _db.ParamGroups.AddAsync(home);
                    await _db.SaveChangesAsync();

                    _logger.LogInformation(_localizer["Seeded Home Page Group Params"]);
                }
            }).GetAwaiter().GetResult();
        }

        private void CreateRoles()
        {
            Task.Run(async () =>
            {
                // Пользователь
                var userRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.User);
                if (userRoleInDb == null)
                {
                    var userRole = new EdoFomsRole(RoleConstants.User, _localizer["User role with basic permissions"]);
                    await _roleManager.CreateAsync(userRole);
                    userRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.User);
                    _logger.LogInformation(_localizer["Seeded User Role"]);

                    foreach (var permission in Permissions.GetUserPermissions())
                    {
                        await _roleManager.AddPermissionClaim(userRoleInDb, permission);
                    }
                }

                // Сотрудник
                var employeeRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.Employee);
                if (employeeRoleInDb == null)
                {
                    var employeeRole = new EdoFomsRole(RoleConstants.Employee, _localizer["Employee role with edit permissions"]);
                    await _roleManager.CreateAsync(employeeRole);
                    employeeRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.Employee);
                    _logger.LogInformation(_localizer["Seeded Employee Role"]);

                    foreach (var permission in Permissions.GetEmployeePermissions())
                    {
                        await _roleManager.AddPermissionClaim(employeeRoleInDb, permission);
                    }
                }

                // Управляющий
                var managerRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.Manager);
                if (managerRoleInDb == null)
                {
                    var managerRole = new EdoFomsRole(RoleConstants.Manager, _localizer["Manager role with org permissions"]);
                    await _roleManager.CreateAsync(managerRole);
                    managerRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.Manager);
                    _logger.LogInformation(_localizer["Seeded Manager Role"]);

                    foreach (var permission in Permissions.GetManagerPermissions())
                    {
                        await _roleManager.AddPermissionClaim(managerRoleInDb, permission);
                    }
                }

                // Руководитель
                var chiefRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.Chief);
                if (chiefRoleInDb == null)
                {
                    var chiefRole = new EdoFomsRole(RoleConstants.Chief, _localizer["Chief role with head permissions"]);
                    await _roleManager.CreateAsync(chiefRole);
                    chiefRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.Chief);
                    _logger.LogInformation(_localizer["Seeded Chief Role"]);

                    foreach (var permission in Permissions.GetChiefPermissions())
                    {
                        await _roleManager.AddPermissionClaim(chiefRoleInDb, permission);
                    }
                }

                // Администратор ЕДО
                var adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.Admin);
                if (adminRoleInDb == null)
                {
                    var adminRole = new EdoFomsRole(RoleConstants.Admin, _localizer["Admin role with full permissions"]);
                    await _roleManager.CreateAsync(adminRole);
                    adminRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.Admin);
                    _logger.LogInformation(_localizer["Seeded Admin Role"]);

                    foreach (var permission in Permissions.GetAdminPermissions())
                    {
                        await _roleManager.AddPermissionClaim(adminRoleInDb, permission);
                    }
                }

                // Работник фонда
                var fundWorkerRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.WorkerOf.Fund);
                if (fundWorkerRoleInDb == null)
                {
                    var fundWorkerRole = new EdoFomsRole(RoleConstants.WorkerOf.Fund, _localizer["Fund Worker role permissions"]);
                    await _roleManager.CreateAsync(fundWorkerRole);
                    fundWorkerRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.WorkerOf.Fund);
                    _logger.LogInformation(_localizer["Seeded Fund Worker Role"]);

                    foreach (var permission in Permissions.GetFundWorkerPermissions())
                    {
                        await _roleManager.AddPermissionClaim(fundWorkerRoleInDb, permission);
                    }
                }

                // Работник СМО
                var smoWorkerRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.WorkerOf.SMO);
                if (smoWorkerRoleInDb == null)
                {
                    var smoWorkerRole = new EdoFomsRole(RoleConstants.WorkerOf.SMO, _localizer["SMO Worker role permissions"]);
                    await _roleManager.CreateAsync(smoWorkerRole);
                    smoWorkerRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.WorkerOf.SMO);
                    _logger.LogInformation(_localizer["Seeded SMO Worker Role"]);

                    foreach (var permission in Permissions.GetSmoWorkerPermissions())
                    {
                        await _roleManager.AddPermissionClaim(smoWorkerRoleInDb, permission);
                    }
                }

                // Работник МО
                var moWorkerRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.WorkerOf.MO);
                if (moWorkerRoleInDb == null)
                {
                    var moWorkerRole = new EdoFomsRole(RoleConstants.WorkerOf.MO, _localizer["MO Worker role permissions"]);
                    await _roleManager.CreateAsync(moWorkerRole);
                    moWorkerRoleInDb = await _roleManager.FindByNameAsync(RoleConstants.WorkerOf.MO);
                    _logger.LogInformation(_localizer["Seeded MO Worker Role"]);

                    foreach (var permission in Permissions.GetMoWorkerPermissions())
                    {
                        await _roleManager.AddPermissionClaim(moWorkerRoleInDb, permission);
                    }
                }
            }).GetAwaiter().GetResult();
        }

        //private void CreateOrgTypes()
        //{
        //    Task.Run(async () =>
        //    {
        //        var undef = new OrganizationType()
        //        {
        //            Id = (int)OrgTypes.Undefined,
        //            Icon = Icons.Material.Outlined.Business,
        //            Color = Color.Default,
        //            Name = "Undefined"
        //        };
        //        var undefInDb = _db.OrganizationTypes.FirstOrDefault(o => o.Name == undef.Name);
        //        if (undefInDb is null) { await _db.OrganizationTypes.AddAsync(undef); }

        //        var fund = new OrganizationType()
        //        {
        //            Id = (int)OrgTypes.Fund,
        //            Icon = Icons.Material.Outlined.HealthAndSafety,
        //            Color = Color.Error,
        //            Name = "Fund"
        //        };
        //        var fundInDb = _db.OrganizationTypes.FirstOrDefault(o => o.Name == fund.Name);
        //        if (fundInDb is null) { await _db.OrganizationTypes.AddAsync(fund); }

        //        var smo = new OrganizationType()
        //        {
        //            Id = (int)OrgTypes.SMO,
        //            Icon = Icons.Material.Outlined.Museum,
        //            Color = Color.Success,
        //            Name = "SMO"
        //        };
        //        var smoInDb = _db.OrganizationTypes.FirstOrDefault(o => o.Name == smo.Name);
        //        if (smoInDb is null) { await _db.OrganizationTypes.AddAsync(smo); }

        //        var mo = new OrganizationType()
        //        {
        //            Id = (int)OrgTypes.MO,
        //            Icon = Icons.Material.Outlined.MedicalServices,
        //            Color = Color.Primary,
        //            Name = "MO"
        //        };
        //        var moInDb = _db.OrganizationTypes.FirstOrDefault(o => o.Name == mo.Name);
        //        if (moInDb is null) { await _db.OrganizationTypes.AddAsync(mo); }

        //        var ca = new OrganizationType()
        //        {
        //            Id = (int)OrgTypes.CA,
        //            Icon = Icons.Material.Outlined.Token,
        //            Color = Color.Secondary,
        //            Name = "CA"
        //        };
        //        var caInDb = _db.OrganizationTypes.FirstOrDefault(o => o.Name == ca.Name);
        //        if (caInDb is null) { await _db.OrganizationTypes.AddAsync(ca); }

        //        var treasury = new OrganizationType()
        //        {
        //            Id = (int)OrgTypes.Treasury,
        //            Icon = Icons.Material.Outlined.AccountBalance,
        //            Color = Color.Warning,
        //            Name = "Treasury"
        //        };
        //        var treasuryInDb = _db.OrganizationTypes.FirstOrDefault(o => o.Name == treasury.Name);
        //        if (treasuryInDb is null) { await _db.OrganizationTypes.AddAsync(treasury); }

        //        var meo = new OrganizationType()
        //        {
        //            Id = (int)OrgTypes.MEO,
        //            Icon = Icons.Material.Outlined.LocalPolice,
        //            Color = Color.Tertiary,
        //            Name = "MEO"
        //        };
        //        var meoInDb = _db.OrganizationTypes.FirstOrDefault(o => o.Name == meo.Name);
        //        if (meoInDb is null) { await _db.OrganizationTypes.AddAsync(meo); }

        //        if (undefInDb is null || fundInDb is null || smoInDb is null || moInDb is null || caInDb is null || treasuryInDb is null || meoInDb is null)
        //        {
        //            await _db.SaveChangesAsync();
        //        }
        //    }).GetAwaiter().GetResult();
        //}

        private void CreateDocTypes()
        {
            Task.Run(async () =>
            {
                var contract = new DocumentType()
                {
                    IsActive = true,
                    Icon = DocIcons.Description,
                    Color = MudBlazor.Color.Default,

                    Short = "Дог",
                    Label = "Договор",
                    Name = "Договор",

                    NameEn = "Contract",
                    Description = "Догово́р (мн. ч. — догово́ры) — соглашение между собой двух или более сторон (субъектов), по какому-либо вопросу с целью установления, изменения или прекращения правовых отношений."
                };

                var contractInDb = _db.DocumentTypes.FirstOrDefault(c => c.Name == contract.Name);
                if (contractInDb == null)
                {
                    await _db.DocumentTypes.AddAsync(contract);
                }

                var agreement = new DocumentType()
                {
                    IsActive = true,
                    Icon = DocIcons.NoteAdd,
                    Color = MudBlazor.Color.Default,

                    Short = "Д/С",
                    Label = "Доп.соглашение",
                    Name = "Дополнительное соглашение",

                    NameEn = "Supplementary agreement",
                    Description = "Документ, в котором содержатся дополнения и изменения, вносимые в ранее заключённый договор."
                };

                var agreementInDb = _db.DocumentTypes.FirstOrDefault(c => c.Name == agreement.Name);
                if (agreementInDb == null)
                {
                    await _db.DocumentTypes.AddAsync(agreement);
                }

                var amek = new DocumentType()
                {
                    IsActive = true,
                    Icon = DocIcons.Receipt,
                    Color = MudBlazor.Color.Default,

                    Short = "АМЭК",
                    Label = "Акт МЭК",
                    Name = "Акт медико-экономического контроля",

                    NameEn = "The act of medical and economic control",
                    Description = ""
                };

                var amekInDb = _db.DocumentTypes.FirstOrDefault(c => c.Name == amek.Name);
                if (amekInDb == null)
                {
                    await _db.DocumentTypes.AddAsync(amek);
                }

                var zrmek = new DocumentType()
                {
                    IsActive = true,
                    Icon = DocIcons.FactCheck,
                    Color = MudBlazor.Color.Default,

                    Short = "ЗпРМЭК",
                    Label = "Закл-e РМЭК",
                    Name = "Заключение по результатам медико-экономического контроля",

                    NameEn = "Conclusion on the results of medical and economic control",
                    Description = ""
                };

                var zrmekInDb = _db.DocumentTypes.FirstOrDefault(c => c.Name == zrmek.Name);
                if (zrmekInDb == null)
                {
                    await _db.DocumentTypes.AddAsync(zrmek);
                }

                var rzrmek = new DocumentType()
                {
                    IsActive = true,
                    Icon = DocIcons.TableChart,
                    Color = MudBlazor.Color.Default,

                    Short = "РЗпРМЭК",
                    Label = "Реестр закл-й РМЭК",
                    Name = "Реестр заключений по результатам медико-экономического контроля",

                    NameEn = "Register of conclusions based on the results of medical and economic control",
                    Description = ""
                };

                var rzrmekInDb = _db.DocumentTypes.FirstOrDefault(c => c.Name == rzrmek.Name);
                if (rzrmekInDb == null)
                {
                    await _db.DocumentTypes.AddAsync(rzrmek);
                }


                if (contractInDb is null || agreementInDb is null || amekInDb is null || zrmekInDb is null || rzrmekInDb is null)
                {
                    await _db.SaveChangesAsync();
                }
            }).GetAwaiter().GetResult();
        }

        private void AddFond()
        {
            Task.Run(async () =>
            {
                var org = new Organization()
                {
                    Inn = "1655417750",
                    Ogrn = "",
                    Name = "ООО \"АЙТИ ПРО100\"",
                    ShortName = "АЙТИ ПРО100",

                    //UserId = user?.Id,
                    //UserSnils = user.Snils,

                    IsPublic = true,
                    Type = OrgTypes.Fund,
                    State = OrgStates.Active,

                    Phone = "+78430000000",
                    Email = "edo@azino.ru",

                    CreatedOn = DateTime.Now
                    //CreatedBy = user.Id
                };

                //Check if Org Exists
                var orgInDb = _db.Organizations.FirstOrDefault(o => o.Inn == org.Inn);
                if (orgInDb == null)
                {
                    await _db.Organizations.AddAsync(org);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                //var chief = new EdoFomsUser
                //    {
                //        OrgId = org.Id,
                //        UserName = "1653006786-12345678900",

                //        InnLe = org.Inn,
                //        Snils = "12345678900",
                //        Inn = "165300678600",

                //        Title = "Chief",
                //        Surname = "Фонд ОМС",
                //        GivenName = "Руководитель",
                //        ProfilePictureDataUrl = "",

                //        BaseRole =UserBaseRoles.Chief,
                //        OrgType = OrgTypes.Fund,
                //        IsActive = true,

                //        Email = "head-edo@fomsrt.ru",
                //        EmailConfirmed = true,
                //        PhoneNumber = "+78430000000",
                //        PhoneNumberConfirmed = true,

                //        CreatedOn = DateTime.Now
                //    };
                //var chiefInDb = await _userManager.FindByNameAsync(chief.UserName);
                //if (chiefInDb == null)
                //{
                //    await _userManager.CreateAsync(chief, UserConstants.DefaultPassword);
                //    await _userManager.AddToRoleAsync(chief, RoleConstants.WorkerOf.Fund);
                //    await _userManager.AddToRoleAsync(chief, RoleConstants.Chief);

                //    _logger.LogInformation(_localizer["Seeded User with Head of the fund Role"]);

                //    var cert = new Certificate()
                //    {
                //        Thumbprint = "01234567890123456789012345678901234-0000",
                //        UserId = chief.Id,
                //        Snils = chief.Snils,

                //        IsActive = true,
                //        SignAllowed = true,

                //        FromDate = DateTime.Now.Date,
                //        TillDate = DateTime.Now.AddYears(2).Date,

                //        CreatedOn = DateTime.Now,
                //        CreatedBy = chief.Id
                //    };

                //    await _db.Certificates.AddAsync(cert);
                //    await _db.SaveChangesAsync();
                //}

                //Check if User Exists
                var admin = new EdoFomsUser
                {
                    OrgId = org.Id,
                    UserName = "1655417750-03821340828", // ORG INNLE + USER SNILS 

                    InnLe = "1655417750",
                    Snils = "03821340828",
                    Inn = "",

                    Title = "Админ ЭДО",
                    Surname = "Закирова",
                    GivenName = "Венера Раушановна",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Admin,
                    OrgType = OrgTypes.Fund,
                    IsActive = true,

                    Email = "kira@azino.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "",
                    PhoneNumberConfirmed = false,

                    CreatedOn = DateTime.Now
                };
                var adminInDb = await _userManager.FindByNameAsync(admin.UserName);
                if (adminInDb == null)
                {
                    await _userManager.CreateAsync(admin, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(admin, RoleConstants.WorkerOf.Fund);
                    await _userManager.AddToRoleAsync(admin, RoleConstants.Admin);

                    _logger.LogInformation(_localizer["Seeded Admin Edm User"]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "2907D270DB3717ABFC76C256C02A7D3071826D70",
                        UserId = admin.Id,
                        Snils = admin.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = new DateTime(2021, 11, 25, 11, 06, 11),
                        TillDate = new DateTime(2023, 02, 25, 11, 06, 11),

                        CreatedOn = DateTime.Now,
                        CreatedBy = admin.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                ////Check if User Exists
                //var manager = new EdoFomsUser
                //{
                //    OrgId = org.Id,
                //    UserName = "1653006786-12345678904", // ORG INNLE + USER SNILS 

                //    InnLe = "1653006786",
                //    Snils = "12345678904",
                //    Inn = "165300678601",

                //    Title = "Manager",
                //    Surname = "Фонд",
                //    GivenName = "Управляющий",
                //    ProfilePictureDataUrl = "",

                //    BaseRole =UserBaseRoles.Admin,
                //    OrgType = OrgTypes.Fund,
                //    IsActive = true,

                //    Email = "manager-edo@fomsrt.ru",
                //    EmailConfirmed = true,
                //    PhoneNumber = "+7 843 000 0001",
                //    PhoneNumberConfirmed = true,

                //    CreatedOn = DateTime.Now
                //};
                //var managerInDb = await _userManager.FindByNameAsync(manager.UserName);
                //if (managerInDb == null)
                //{
                //    await _userManager.CreateAsync(manager, UserConstants.DefaultPassword);
                //    await _userManager.AddToRoleAsync(manager, RoleConstants.WorkerOf.Fund);
                //    await _userManager.AddToRoleAsync(manager, RoleConstants.Manager);

                //    _logger.LogInformation(_localizer["Seeded Admin Edm User"]);

                //    var cert = new Certificate()
                //    {
                //        Thumbprint = "01234567890123456789012345678901234-0004",
                //        UserId = manager.Id,
                //        Snils = manager.Snils,

                //        IsActive = true,
                //        SignAllowed = false,

                //        FromDate = DateTime.Now.AddYears(-1).Date,
                //        TillDate = DateTime.Now.AddYears(1).Date,

                //        CreatedOn = DateTime.Now,
                //        CreatedBy = manager.Id
                //    };

                //    await _db.Certificates.AddAsync(cert);
                //    await _db.SaveChangesAsync();
                //}

                ////Check if User Exists
                //var employee = new EdoFomsUser
                //{
                //    OrgId = org.Id,
                //    UserName = "1653006786-12345678902",

                //    InnLe = "1653006786",
                //    Snils = "12345678902",
                //    Inn = "165300678602",

                //    Title = "initiator",
                //    Surname = "Фонд",
                //    GivenName = "Сотрудник",
                //    ProfilePictureDataUrl = "",

                //    BaseRole =UserBaseRoles.Employee,
                //    OrgType = OrgTypes.Fund,
                //    IsActive = true,

                //    Email = "initiator-edo@fomsrt.ru",
                //    EmailConfirmed = true,
                //    PhoneNumber = "+7 843 000 0002",
                //    PhoneNumberConfirmed = true,

                //    CreatedOn = DateTime.Now
                //};
                //var employeeInDb = await _userManager.FindByNameAsync(employee.UserName);
                //if (employeeInDb == null)
                //{
                //    await _userManager.CreateAsync(employee, UserConstants.DefaultPassword);
                //    await _userManager.AddToRoleAsync(employee, RoleConstants.WorkerOf.Fund);
                //    await _userManager.AddToRoleAsync(employee, RoleConstants.Employee);
                //    _logger.LogInformation(_localizer["Seeded User with Employee Role"]);

                //    var cert = new Certificate()
                //    {
                //        Thumbprint = "01234567890123456789012345678901234-0002",
                //        UserId = employee.Id,
                //        Snils = employee.Snils,

                //        IsActive = true,
                //        SignAllowed = false,

                //        FromDate = DateTime.Now.AddYears(-1).Date,
                //        TillDate = DateTime.Now.AddYears(1).Date,

                //        CreatedOn = DateTime.Now,
                //        CreatedBy = employee.Id
                //    };

                //    await _db.Certificates.AddAsync(cert);
                //    await _db.SaveChangesAsync();
                //}

                ////Check if User Exists
                //var user = new EdoFomsUser
                //    {
                //        OrgId = org.Id,
                //        UserName = "1653006786-12345678903",

                //        InnLe = "1653006786",
                //        Snils = "12345678903",
                //        Inn = "165300678603",

                //        Title = "Coordinator",
                //        Surname = "Фонд",
                //        GivenName = "Пользователь",
                //        ProfilePictureDataUrl = "",

                //        BaseRole =UserBaseRoles.User,
                //        OrgType = OrgTypes.Fund,
                //        IsActive = true,

                //        Email = "coordinator-edo@fomsrt.ru",
                //        EmailConfirmed = true,
                //        PhoneNumber = "+7 843 000 0003",
                //        PhoneNumberConfirmed = true,

                //        CreatedOn = DateTime.Now
                //    };
                //var userInDb = await _userManager.FindByNameAsync(user.UserName);
                //if (userInDb == null)
                //{
                //    await _userManager.CreateAsync(user, UserConstants.DefaultPassword);
                //    await _userManager.AddToRoleAsync(user, RoleConstants.WorkerOf.Fund);
                //    await _userManager.AddToRoleAsync(user, RoleConstants.User);
                //    _logger.LogInformation(_localizer["Seeded User with Coordinator Role."]);

                //    var cert = new Certificate()
                //    {
                //        Thumbprint = "01234567890123456789012345678901234-0003",
                //        UserId = user.Id,
                //        Snils = user.Snils,

                //        IsActive = true,
                //        SignAllowed = false,

                //        FromDate = DateTime.Now.AddMonths(-18).Date,
                //        TillDate = DateTime.Now.AddYears(6).Date,

                //        CreatedOn = DateTime.Now,
                //        CreatedBy = user.Id
                //    };

                //    await _db.Certificates.AddAsync(cert);
                //    await _db.SaveChangesAsync();
                //}
            }).GetAwaiter().GetResult();
        }

        private void AddSmo1()
        {
            Task.Run(async () =>
            {
                //Check if Org Exists
                var org = new Organization()
                {
                    Inn = "1600000100",
                    Ogrn = "1000000000000",
                    Name = "СМО 1",

                    //UserId = userInDb.Id,
                    //UserSnils = user.Snils,

                    IsPublic = true,
                    Type = OrgTypes.SMO,
                    State = OrgStates.Active,

                    Phone = "+78430000100",
                    Email = "info@smo-1-rt.ru",

                    CreatedOn = DateTime.Now
                    //CreatedBy = user.Id
                };
                var orgInDb = _db.Organizations.FirstOrDefault(o => o.Inn == org.Inn);
                if (orgInDb == null)
                {
                    await _db.Organizations.AddAsync(org);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var chief = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600000100-12345678910",
                    Snils = "12345678910",
                    Inn = "160000010000",

                    Title = "Chief",
                    Surname = "СМО 1",
                    GivenName = "Руководитель СМО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Chief,
                    OrgType = OrgTypes.SMO,
                    IsActive = true,

                    Email = "chief@smo-1-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430000100",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var chiefInDb = await _userManager.FindByNameAsync(chief.UserName);
                if (chiefInDb == null)
                {
                    await _userManager.CreateAsync(chief, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(chief, RoleConstants.WorkerOf.SMO);
                    await _userManager.AddToRoleAsync(chief, RoleConstants.Chief);
                    _logger.LogInformation(_localizer["Seeded User with Chief Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-0100",
                        UserId = chief.Id,
                        Snils = chief.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = chief.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var manager = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600000100-12345678911",
                    Snils = "12345678911",
                    Inn = "160000010001",

                    Title = "Manager",
                    Surname = "СМО 1",
                    GivenName = "Управляющий СМО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Manager,
                    OrgType = OrgTypes.SMO,
                    IsActive = true,

                    Email = "manager@smo-1-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430000101",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var managerInDb = await _userManager.FindByNameAsync(manager.UserName);
                if (managerInDb == null)
                {
                    await _userManager.CreateAsync(manager, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(manager, RoleConstants.WorkerOf.SMO);
                    await _userManager.AddToRoleAsync(manager, RoleConstants.Manager);
                    _logger.LogInformation(_localizer["Seeded User with Manager Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-0101",
                        UserId = manager.Id,
                        Snils = manager.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = manager.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var employee = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600000100-12345678912",
                    Snils = "12345678912",
                    Inn = "160000010002",

                    Title = "User",
                    Surname = "СМО 1",
                    GivenName = "Сотрудник СМО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Employee,
                    OrgType = OrgTypes.SMO,
                    IsActive = true,

                    Email = "User@smo-1-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430000102",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var employeeInDb = await _userManager.FindByNameAsync(employee.UserName);
                if (employeeInDb == null)
                {
                    await _userManager.CreateAsync(employee, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(employee, RoleConstants.WorkerOf.SMO);
                    await _userManager.AddToRoleAsync(employee, RoleConstants.Employee);
                    _logger.LogInformation(_localizer["Seeded User with Employee Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-0102",
                        UserId = employee.Id,
                        Snils = employee.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = employee.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var user = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600000100-12345678913",
                    Snils = "12345678913",
                    Inn = "160000010003",

                    Title = "Employee",
                    Surname = "СМО 1",
                    GivenName = "Пользователь СМО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.User,
                    OrgType = OrgTypes.SMO,
                    IsActive = true,

                    Email = "employee@smo-1-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430000103",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var userInDb = await _userManager.FindByNameAsync(user.UserName);
                if (userInDb == null)
                {
                    await _userManager.CreateAsync(user, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(user, RoleConstants.WorkerOf.SMO);
                    await _userManager.AddToRoleAsync(user, RoleConstants.User);
                    _logger.LogInformation(_localizer["Seeded User with User Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-0103",
                        UserId = employee.Id,
                        Snils = employee.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = employee.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }
            }).GetAwaiter().GetResult();
        }

        private void AddSmo2()
        {
            Task.Run(async () =>
            {
                //Check if Org Exists
                var org = new Organization()
                {
                    Inn = "1600000200",
                    Ogrn = "2000000000000",
                    Name = "СМО 2",

                    //UserId = user?.Id ?? "",
                    //UserSnils = user.Snils,

                    IsPublic = true,
                    Type = OrgTypes.SMO,
                    State = OrgStates.Active,

                    Phone = "+78430000200",
                    Email = "info@smo-2-rt.ru",

                    CreatedOn = DateTime.Now,
                    //CreatedBy = user.Id
                };
                var orgInDb = _db.Organizations.FirstOrDefault(o => o.Inn == org.Inn);
                if (orgInDb == null)
                {
                    await _db.Organizations.AddAsync(org);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var chief = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600000200-12345678920",
                    Snils = "12345678920",
                    Inn = "160000020000",

                    Title = "Chief",
                    Surname = "СМО 2",
                    GivenName = "Руководитель СМО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Chief,
                    OrgType = OrgTypes.SMO,
                    IsActive = true,

                    Email = "chief@smo-2-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430000200",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var chiefInDb = await _userManager.FindByNameAsync(chief.UserName);
                if (chiefInDb == null)
                {
                    await _userManager.CreateAsync(chief, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(chief, RoleConstants.WorkerOf.SMO);
                    await _userManager.AddToRoleAsync(chief, RoleConstants.Chief);
                    _logger.LogInformation(_localizer["Seeded User with Chief Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-0200",
                        UserId = chief.Id,
                        Snils = chief.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = chief.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var manager = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600000200-12345678921",
                    Snils = "12345678921",
                    Inn = "160000020001",

                    Title = "Manager",
                    Surname = "СМО 2",
                    GivenName = "Управляющий СМО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Manager,
                    OrgType = OrgTypes.SMO,
                    IsActive = true,

                    Email = "manager@smo-2-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430000201",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var managerInDb = await _userManager.FindByNameAsync(manager.UserName);
                if (managerInDb == null)
                {
                    await _userManager.CreateAsync(manager, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(manager, RoleConstants.Manager);
                    await _userManager.AddToRoleAsync(manager, RoleConstants.WorkerOf.SMO);
                    _logger.LogInformation(_localizer["Seeded User with Manager Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-0201",
                        UserId = manager.Id,
                        Snils = manager.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = manager.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var employee = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600000200-12345678923",
                    Snils = "12345678923",
                    Inn = "160000020003",

                    Title = "Employee",
                    Surname = "СМО 2",
                    GivenName = "Сотрудник СМО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Employee,
                    OrgType = OrgTypes.SMO,
                    IsActive = true,

                    Email = "employee@smo-2-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430000203",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var employeeInDb = await _userManager.FindByNameAsync(employee.UserName);
                if (employeeInDb == null)
                {
                    await _userManager.CreateAsync(employee, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(employee, RoleConstants.WorkerOf.SMO);
                    await _userManager.AddToRoleAsync(employee, RoleConstants.Employee);
                    _logger.LogInformation(_localizer["Seeded User with Employee Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-0203",
                        UserId = employee.Id,
                        Snils = employee.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = employee.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var user = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600000200-12345678922",
                    Snils = "12345678922",
                    Inn = "160000020002",

                    Title = "User",
                    Surname = "СМО 2",
                    GivenName = "Пользователь СМО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.User,
                    OrgType = OrgTypes.SMO,
                    IsActive = true,

                    Email = "User@smo-2-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430000202",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var userInDb = await _userManager.FindByNameAsync(user.UserName);
                if (userInDb == null)
                {
                    await _userManager.CreateAsync(user, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(user, RoleConstants.WorkerOf.SMO);
                    await _userManager.AddToRoleAsync(user, RoleConstants.User);
                    _logger.LogInformation(_localizer["Seeded User with User Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-0202",
                        UserId = user.Id,
                        Snils = user.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = user.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

            }).GetAwaiter().GetResult();
        }

        private void AddSmo3()
        {
            Task.Run(async () =>
            {
                //Check if Org Exists
                var org = new Organization()
                {
                    Inn = "1600000300",
                    Ogrn = "3000000000000",
                    Name = "СМО 3",

                    //UserId = userInDb?.Id ?? user?.Id,
                    //UserSnils = user.Snils,

                    IsPublic = true,
                    Type = OrgTypes.SMO,
                    State = OrgStates.Active,

                    Phone = "+78430000300",
                    Email = "info@smo-3-rt.ru",

                    CreatedOn = DateTime.Now,
                    //CreatedBy = user.Id
                };
                var orgInDb = _db.Organizations.FirstOrDefault(o => o.Inn == org.Inn);
                if (orgInDb == null)
                {
                    await _db.Organizations.AddAsync(org);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var chief = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600000300-12345678930",
                    Snils = "12345678930",
                    Inn = "160000030000",

                    Title = "Chief",
                    Surname = "СМО 3",
                    GivenName = "Руководитель СМО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Chief,
                    OrgType = OrgTypes.SMO,
                    IsActive = true,

                    Email = "chief@smo-3-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430000300",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var chiefInDb = await _userManager.FindByNameAsync(chief.UserName);
                if (chiefInDb == null)
                {
                    await _userManager.CreateAsync(chief, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(chief, RoleConstants.WorkerOf.SMO);
                    await _userManager.AddToRoleAsync(chief, RoleConstants.Chief);
                    _logger.LogInformation(_localizer["Seeded User with Chief Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-0300",
                        UserId = chief.Id,
                        Snils = chief.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = chief.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var manager = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600000300-12345678931",
                    Snils = "12345678931",
                    Inn = "160000030001",

                    Title = "Manager",
                    Surname = "СМО 3",
                    GivenName = "Управляющий СМО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Manager,
                    OrgType = OrgTypes.SMO,
                    IsActive = true,

                    Email = "manager@smo-3-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430000301",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var managerInDb = await _userManager.FindByNameAsync(manager.UserName);
                if (managerInDb == null)
                {
                    await _userManager.CreateAsync(manager, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(manager, RoleConstants.WorkerOf.SMO);
                    await _userManager.AddToRoleAsync(manager, RoleConstants.Manager);
                    _logger.LogInformation(_localizer["Seeded User with Manager Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-0301",
                        UserId = manager.Id,
                        Snils = manager.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = manager.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var employee = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600000300-12345678933",
                    Snils = "12345678933",
                    Inn = "160000030003",

                    Title = "Employee",
                    Surname = "СМО 3",
                    GivenName = "Сотрудник СМО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Employee,
                    OrgType = OrgTypes.SMO,
                    IsActive = true,

                    Email = "employee@smo-3-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430000303",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var employeeInDb = await _userManager.FindByNameAsync(employee.UserName);
                if (employeeInDb == null)
                {
                    await _userManager.CreateAsync(employee, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(employee, RoleConstants.WorkerOf.SMO);
                    await _userManager.AddToRoleAsync(employee, RoleConstants.Employee);
                    _logger.LogInformation(_localizer["Seeded User with Employee Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-0303",
                        UserId = employee.Id,
                        Snils = employee.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = employee.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var user = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600000300-12345678932",
                    Snils = "12345678932",
                    Inn = "160000030002",

                    Title = "User",
                    Surname = "СМО 3",
                    GivenName = "Пользователь СМО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.User,
                    OrgType = OrgTypes.SMO,
                    IsActive = true,

                    Email = "User@smo-3-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430000302",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var userInDb = await _userManager.FindByNameAsync(user.UserName);
                if (userInDb == null)
                {
                    await _userManager.CreateAsync(user, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(user, RoleConstants.WorkerOf.SMO);
                    await _userManager.AddToRoleAsync(user, RoleConstants.User);
                    _logger.LogInformation(_localizer["Seeded User with User Role"]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-0302",
                        UserId = user.Id,
                        Snils = user.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = user.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

            }).GetAwaiter().GetResult();
        }

        private void AddMo1()
        {
            Task.Run(async () =>
            {
                //Check if Org Exists
                var org = new Organization()
                {
                    Inn = "1600100000",
                    Ogrn = "7000000000000",
                    Name = "МО 1",

                    //UserId = user?.Id ?? "",
                    //UserSnils = user.Snils,

                    IsPublic = true,
                    Type = OrgTypes.MO,
                    State = OrgStates.Active,

                    Phone = "+78430010000",
                    Email = "info@mo-1-rt.ru",

                    CreatedOn = DateTime.Now
                    //CreatedBy = user.Id
                };
                var orgInDb = _db.Organizations.FirstOrDefault(o => o.Inn == org.Inn);
                if (orgInDb == null)
                {
                    await _db.Organizations.AddAsync(org);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var chief = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600100000-12345678100",
                    Snils = "12345678100",
                    Inn = "160010000000",

                    Title = "Chief",
                    Surname = "МО 1",
                    GivenName = "Руководитель MO",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Chief,
                    OrgType = OrgTypes.MO,
                    IsActive = true,

                    Email = "chief@mo-1-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430010000",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var chiefInDb = await _userManager.FindByNameAsync(chief.UserName);
                if (chiefInDb == null)
                {
                    await _userManager.CreateAsync(chief, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(chief, RoleConstants.WorkerOf.MO);
                    await _userManager.AddToRoleAsync(chief, RoleConstants.Chief);
                    _logger.LogInformation(_localizer["Seeded User with Chief Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-1000",
                        UserId = chief.Id,
                        Snils = chief.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = chief.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var manager = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600100000-12345678101",
                    Snils = "12345678101",
                    Inn = "160010000001",

                    Title = "Manager",
                    Surname = "МО 1",
                    GivenName = "Управляющий МО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Manager,
                    OrgType = OrgTypes.MO,
                    IsActive = true,

                    Email = "manager@mo-1-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430010001",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var managerInDb = await _userManager.FindByNameAsync(manager.UserName);
                if (managerInDb == null)
                {
                    await _userManager.CreateAsync(manager, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(manager, RoleConstants.WorkerOf.MO);
                    await _userManager.AddToRoleAsync(manager, RoleConstants.Manager);
                    _logger.LogInformation(_localizer["Seeded User with Manager Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-1001",
                        UserId = manager.Id,
                        Snils = manager.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = manager.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var employee = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600100000-12345678102",
                    Snils = "12345678103",
                    Inn = "160010000003",

                    Title = "Employee",
                    Surname = "МО 1",
                    GivenName = "Сотрудник МО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Employee,
                    OrgType = OrgTypes.MO,
                    IsActive = true,

                    Email = "employee@mo-1-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430010003",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var employeeInDb = await _userManager.FindByNameAsync(employee.UserName);
                if (employeeInDb == null)
                {
                    await _userManager.CreateAsync(employee, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(employee, RoleConstants.WorkerOf.MO);
                    await _userManager.AddToRoleAsync(employee, RoleConstants.Employee);
                    _logger.LogInformation(_localizer["Seeded User with Employee Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-1003",
                        UserId = employee.Id,
                        Snils = employee.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = employee.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var user = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600100000-12345678103",
                    Snils = "12345678102",
                    Inn = "160010000002",

                    Title = "User",
                    Surname = "МО 1",
                    GivenName = "Пользователь МО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.User,
                    OrgType = OrgTypes.MO,
                    IsActive = true,

                    Email = "User@mo-1-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430010002",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var userInDb = await _userManager.FindByNameAsync(user.UserName);
                if (userInDb == null)
                {
                    await _userManager.CreateAsync(user, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(user, RoleConstants.WorkerOf.MO);
                    await _userManager.AddToRoleAsync(user, RoleConstants.User);
                    _logger.LogInformation(_localizer["Seeded User with User Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-1002",
                        UserId = user.Id,
                        Snils = user.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = user.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }
            }).GetAwaiter().GetResult();
        }

        private void AddMo2()
        {
            Task.Run(async () =>
            {
                //Check if Org Exists
                var org = new Organization()
                {
                    Inn = "1600200000",
                    Ogrn = "8000000000000",
                    Name = "МО 2",

                    //UserId = user?.Id ?? "",
                    //UserSnils = user.Snils,

                    IsPublic = true,
                    Type = OrgTypes.MO,
                    State = OrgStates.Active,

                    Phone = "+78430020000",
                    Email = "info@mo-2-rt.ru",

                    CreatedOn = DateTime.Now,
                    //CreatedBy = user.Id
                };
                var orgInDb = _db.Organizations.FirstOrDefault(o => o.Inn == org.Inn);
                if (orgInDb == null)
                {
                    await _db.Organizations.AddAsync(org);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var chief = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600200000-12345678200",
                    Snils = "12345678200",
                    Inn = "160020000000",

                    Title = "Chief",
                    Surname = "МО 2",
                    GivenName = "Руководитель MO",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Chief,
                    OrgType = OrgTypes.MO,
                    IsActive = true,

                    Email = "chief@mo-2-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430020000",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var chiefInDb = await _userManager.FindByNameAsync(chief.UserName);
                if (chiefInDb == null)
                {
                    await _userManager.CreateAsync(chief, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(chief, RoleConstants.WorkerOf.MO);
                    await _userManager.AddToRoleAsync(chief, RoleConstants.Chief);
                    _logger.LogInformation(_localizer["Seeded User with Chief Role"]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-2000",
                        UserId = chief.Id,
                        Snils = chief.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = chief.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var manager = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600200000-12345678201",
                    Snils = "12345678201",
                    Inn = "160020000001",

                    Title = "Manager",
                    Surname = "МО 2",
                    GivenName = "Управляющий МО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Manager,
                    OrgType = OrgTypes.MO,
                    IsActive = true,

                    Email = "manager@mo-2-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430020001",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var managerInDb = await _userManager.FindByNameAsync(manager.UserName);
                if (managerInDb == null)
                {
                    await _userManager.CreateAsync(manager, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(manager, RoleConstants.WorkerOf.MO);
                    await _userManager.AddToRoleAsync(manager, RoleConstants.Manager);
                    _logger.LogInformation(_localizer["Seeded User with Manager Role"]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-2001",
                        UserId = manager.Id,
                        Snils = manager.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = manager.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var employee = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600200000-12345678203",
                    Snils = "12345678203",
                    Inn = "160020000003",

                    Surname = "МО 2",
                    GivenName = "Сотрудник МО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.Employee,
                    OrgType = OrgTypes.MO,
                    IsActive = true,

                    Email = "employee@mo-2-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430020003",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var employeeInDb = await _userManager.FindByNameAsync(employee.UserName);
                if (employeeInDb == null)
                {
                    await _userManager.CreateAsync(employee, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(employee, RoleConstants.WorkerOf.MO);
                    await _userManager.AddToRoleAsync(employee, RoleConstants.Employee);
                    _logger.LogInformation(_localizer["Seeded User with Employee Role"]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-2003",
                        UserId = employee.Id,
                        Snils = employee.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = employee.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }

                //Check if User Exists
                var user = new EdoFomsUser
                {
                    OrgId = org.Id,
                    InnLe = org.Inn,

                    UserName = "1600200000-12345678202",
                    Snils = "12345678202",
                    Inn = "160020000002",

                    Title = "User",
                    Surname = "МО 2",
                    GivenName = "Пользователь МО",
                    ProfilePictureDataUrl = "",

                    BaseRole = UserBaseRoles.User,
                    OrgType = OrgTypes.MO,
                    IsActive = true,

                    Email = "User@mo-2-rt.ru",
                    EmailConfirmed = true,
                    PhoneNumber = "+78430020002",
                    PhoneNumberConfirmed = true,

                    CreatedOn = DateTime.Now
                };
                var userInDb = await _userManager.FindByNameAsync(user.UserName);
                if (userInDb == null)
                {
                    await _userManager.CreateAsync(user, UserConstants.DefaultPassword);
                    await _userManager.AddToRoleAsync(user, RoleConstants.WorkerOf.MO);
                    await _userManager.AddToRoleAsync(user, RoleConstants.User);
                    _logger.LogInformation(_localizer["Seeded User with User Role."]);

                    var cert = new Certificate()
                    {
                        Thumbprint = "01234567890123456789012345678901234-2002",
                        UserId = user.Id,
                        Snils = user.Snils,

                        IsActive = true,
                        SignAllowed = true,

                        FromDate = DateTime.Now.Date,
                        TillDate = DateTime.Now.AddYears(2).Date,

                        CreatedOn = DateTime.Now,
                        CreatedBy = user.Id
                    };

                    await _db.Certificates.AddAsync(cert);
                    await _db.SaveChangesAsync();
                }
            }).GetAwaiter().GetResult();
        }

        //private void AddUserWoOrg()
        //{
        //    Task.Run(async () =>
        //    {
        //        //Check if User Exists
        //        var user = new EdoFomsUser
        //        {
        //            UserName = "1616000000-12345670000",

        //            InnLe = "1616000000",
        //            Snils = "12345670000",
        //            Inn = "161600000000",

        //            Surname = "Без организации",
        //            GivenName = "Пользователь",
        //            ProfilePictureDataUrl = "",

        //            BaseRole =UserBaseRoles.User,
        //            OrgType = OrgTypes.Undefined,
        //            IsActive = true,

        //            Email = "user@rt.ru",
        //            EmailConfirmed = true,
        //            PhoneNumber = "+78431600000",
        //            PhoneNumberConfirmed = true,

        //            CreatedOn = DateTime.Now
        //        };

        //        var userInDb = await _userManager.FindByNameAsync(user.UserName);
        //        if (userInDb == null)
        //        {
        //            await _userManager.CreateAsync(user, UserConstants.DefaultPassword);
        //            await _userManager.AddToRoleAsync(user, RoleConstants.BasicRole);
        //            _logger.LogInformation(_localizer["Seeded User with Basic Role."]);

        //            var cert = new Certificate()
        //            {
        //                Thumbprint = "01234567890123456789012345678901234-X000",
        //                UserId = user?.Id ?? "",
        //                Snils = user.Snils,

        //                IsActive = false,
        //                SignAllowed = false,

        //                FromDate = DateTime.Now.Date,
        //                TillDate = DateTime.Now.AddYears(2).Date,

        //                CreatedOn = DateTime.Now,
        //                CreatedBy = user.Id
        //            };

        //            await _db.Certificates.AddAsync(cert);
        //            await _db.SaveChangesAsync();
        //        }
        //    }).GetAwaiter().GetResult();
        //}
    }
}