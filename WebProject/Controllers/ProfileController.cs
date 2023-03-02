using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

using WebProject.Domain.Models;
using WebProject.Models;
using WebProject.Persistence.Contexts;
using WebProject.Persistence.Repositories;

namespace WebProject.Controllers
{
    public class ProfileController : Controller
    {
        private BiblioDbContext _context;
        IWebHostEnvironment _appEnvironment;
        UserRepository userRepository;
        RoleRepository roleRepository;
        UserRoleRepository userRoleRepository;
        List<User> UsersList = new List<User>();
        IEnumerable<UserRole> UserRoles = new List<UserRole>();

        public ProfileController(BiblioDbContext context, IWebHostEnvironment appEnvironment)
        {
            this._context = context;
            this._appEnvironment = appEnvironment;
            this.userRepository = new UserRepository(_context);
            this.roleRepository = new RoleRepository(_context);
            this.userRoleRepository = new UserRoleRepository(_context);
            this.UsersList = _context.Users.ToList();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {

                    byte[] PasswordSalt = new byte[128 / 8];
                    using (var rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(PasswordSalt);
                    }

                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: model.Password, salt: PasswordSalt, prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));

                    user = new User() { SurName = model.SurName, Name = model.Name, Email = model.Email, PasswordSalt = PasswordSalt, PasswordHash = hashed, RegistrationDate = DateTime.Now, LastSiteAccessDate = DateTime.Now };


                    await userRepository.CreateAsync(user);
                    await userRepository.SaveAsync();

                    user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                    if (model.Avatar != null)
                    {
                        string path = "/Avatars/" + user.UserId + ".png";
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await model.Avatar.CopyToAsync(fileStream);
                        }
                        user.AvatarPath = path;
                    }

                    if (model.MiddleName != null)
                    {
                        Console.WriteLine(model.MiddleName);
                        user.MiddleName = model.MiddleName;
                    }

                    if (model.StudentIDNumber != "")
                    {
                        user.StudentIDNumber = model.StudentIDNumber;
                    }

                    await userRepository.UpdateAsync(user);
                    await userRepository.SaveAsync();

                    Role UserRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Пользователь");
                    UserRole newUserRole = new UserRole { UserID = user.UserId, RoleID = UserRole.RoleID, Role = UserRole };

                    await userRoleRepository.CreateAsync(newUserRole);
                    await userRoleRepository.SaveAsync();

                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        public async Task<IActionResult> Exit()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                await userRepository.SaveAsync();
                if (user != null)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: model.Password, salt: user.PasswordSalt, prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));
                    if (user.PasswordHash == hashed)
                    {
                        await Authenticate(user);
                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim("bool",$"{user.ConfirmedStudentId}")
            };

            List<UserRole> userRolesList = await _context.UserRoles.ToListAsync();
            IEnumerable<UserRole> userRoles = userRolesList.Where(u => u.UserID == user.UserId);


            foreach (var item in userRoles)
            {
                Role role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleID == item.RoleID);
                if (role.RoleName == "Администратор" || role.RoleName == "Пользователь" || role.RoleName == "Модератор" || role.RoleName == "Преподаватель")
                {
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.RoleName));
                }
            }

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            user.LastSiteAccessDate = DateTime.Now;

            await userRepository.UpdateAsync(user);
            await userRepository.SaveAsync();

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Profile()
        {
            if (User.Identity.IsAuthenticated)
            {
                Console.WriteLine(User);
                IEnumerable<User> users = await userRepository.ListAsync();
                User user = UsersList.FirstOrDefault(u => u.Email == User.Identity.Name);

                ProfileViewModel userViewModel = new ProfileViewModel() { UserName = user.Name, UserSurName = user.SurName, Email = user.Email, RegistrationDate = user.RegistrationDate, LastSiteAccessDate = user.LastSiteAccessDate };

                userViewModel.AvatarPath = user.AvatarPath == null ? "/Avatars/StandartAvatar.png" : user.AvatarPath;

                if (user.MiddleName != null)
                {
                    userViewModel.MiddleName = user.MiddleName;
                }
                if (user.StudentIDNumber != null)
                {
                    userViewModel.StudentIDNumber = user.StudentIDNumber;
                }
                return View(userViewModel);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ListOfUsers(string? search)
        {
            if (User.IsInRole("Администратор"))
            {
                if (search == null)
                {
                    return View(UsersList);
                }
                else
                {
                    IEnumerable<User> searchedUsers = UsersList.Where(u => u.ConfirmedStudentId == false);
                    return View(searchedUsers);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ChangeUser(int userId)
        {
            if (User.IsInRole("Администратор"))
            {
                User user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == userId);
                List<UserRole> userRolesList = _context.UserRoles.ToList();
                IEnumerable<UserRole> userRoles = userRolesList.Where(u => u.UserID == user.UserId);
                foreach (var role in userRoles)
                {
                    if (role.Role.RoleName == "")
                    {
                        Role newRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleID == role.RoleID);
                        role.Role = newRole;
                        await userRoleRepository.UpdateAsync(role);
                        await userRoleRepository.SaveAsync();
                    }
                }
                UserInfo userInfo = new UserInfo { UserId = user.UserId, AvatarPath = user.AvatarPath, Email = user.Email, SurName = user.SurName, MiddleName = user.MiddleName, Name = user.Name, UserRoles = userRoles, StudentIDNumber = user.StudentIDNumber, ConfirmedStudentId = user.ConfirmedStudentId };
                return View(userInfo);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(UserInfo model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == model.UserId);
                Console.WriteLine(model.UserId);
                if (user != null)
                {
                    user.SurName = model.SurName;
                    user.Name = model.Name;
                    user.MiddleName = model.MiddleName;
                    user.Email = model.Email;
                    user.ConfirmedStudentId = model.ConfirmedStudentId;
                    user.StudentIDNumber = model.StudentIDNumber;
                    if (model.Avatar != null)
                    {
                        FileInfo oldAvatar = new FileInfo(_appEnvironment.WebRootPath + user.UserId);
                        oldAvatar.Delete();
                        string path = "/Avatars/" + user.UserId + ".png";
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await model.Avatar.CopyToAsync(fileStream);
                        }
                        user.AvatarPath = path;
                    }
                    await userRepository.UpdateAsync(user);
                    await userRepository.SaveAsync();

                    return RedirectToAction("ListOfUsers", "Profile");
                }
            }
            return View(model);
        }

        public IActionResult ChangeRoles(int userId)
        {
            ViewBag.UserRoles = new SelectList(UserRoles, "UserRoleID", "Role");
            return View();
        }

        public async Task<IActionResult> EditProfile()
        {
            if (User.Identity.IsAuthenticated)
            {
                IEnumerable<User> users = await userRepository.ListAsync();
                User userModel = UsersList.FirstOrDefault(u => u.Email == User.Identity.Name);
                EditProfile model = new EditProfile() { SurName = userModel.SurName, Name = userModel.Name, Email = userModel.Email, MiddleName = userModel.MiddleName,  Avatar = null };
                if (!userModel.ConfirmedStudentId)
                    model.StudentIDNumber = userModel.StudentIDNumber;
                return View(model);
            }
            return RedirectToAction("Index", "Home");   
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfile model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null)
                {
                    if (model.SurName != "")
                    {
                        user.SurName = model.SurName;
                    }
                    if (model.Name != "")
                    {
                        user.Name = model.Name;
                    }
                    if (model.MiddleName != "")
                    {
                        user.MiddleName = model.MiddleName;
                    }
                    if (model.MiddleName != "")
                    {
                        user.Email = model.Email;
                    }
                    if (model.StudentIDNumber != "" || user.ConfirmedStudentId==false)
                    {
                        user.StudentIDNumber = model.StudentIDNumber;
                    }
                    if (model.Avatar != null)
                    {
                        FileInfo oldAvatar = new FileInfo(_appEnvironment.WebRootPath + user.UserId);
                        oldAvatar.Delete();
                        string path = "/Avatars/" + user.UserId + ".png";
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await model.Avatar.CopyToAsync(fileStream);
                        }
                        user.AvatarPath = path;
                    }
                    await userRepository.UpdateAsync(user);
                    await userRepository.SaveAsync();

                    return RedirectToAction("Profile", "Profile");
                }
            }
            return View(model);
        }
    }
}