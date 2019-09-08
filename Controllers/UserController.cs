﻿using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Fakebook.Models;
using Fakebook.Services;
using System.Collections.Generic;

namespace Fakebook.Controllers
{
    public class UserController : Controller
    {
        private readonly WallService wallService;
        private readonly UserService userService;
        private readonly GroupService groupService;
        private readonly BlogService blogService;
        private readonly StoreService storeService;
        public UserController(WallService wallService, UserService userService, GroupService groupService, BlogService blogService, StoreService storeService)
        {
            this.wallService = wallService;
            this.userService = userService;
            this.groupService = groupService;
            this.blogService = blogService;
            this.storeService = storeService;
        }

        [HttpGet("/User/{id}", Name = "ViewUser")]
        public IActionResult Index(int id)
        {
            ViewBag.Me = userService.GetPersonBasedOnId(User.Identity.GetPersonId());
            if (User.Identity.IsAuthenticated)
            {
                // Show the user
                var person = userService.GetPersonBasedOnId(id);
                //if (person.PersonId == User.Identity.GetPersonId())
                //{
                //    return RedirectToAction("Index", "Home");
                //}

                person.WallPosts = wallService.GetWallPosts(person.PersonId);
                ViewBag.PersonId = User.Identity.GetPersonId();
                return View(person);

            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        // Used for adding wall post
        [HttpPost]
        public IActionResult AddWallPost(int id, WallPost wp)
        {
            wp.PosterName = User.Identity.GetName();
            wp.PosterId = User.Identity.GetPersonId();
            wp.DatePosted = DateTime.Now;
            wallService.AddWallPost(wp);

            return RedirectToAction(nameof(Index), new { id = id });
        }

        // Used for replying to posts
        [HttpPost]
        public IActionResult AddReplyPost(int id, ReplyPost rp)
        {
            rp.PosterId = User.Identity.GetPersonId();
            rp.PosterName = User.Identity.GetName();
            rp.DatePosted = DateTime.Now;
            wallService.AddReplyPost(rp);

            return RedirectToAction(nameof(Index), new { id = id });
        }

        // USed for editting a wall post
        [HttpGet("User/{id}/EditPost/{WallPostId}", Name = "UserEditWallPost")]
        public IActionResult EditWallPost(int id, int WallPostId)
        {
            ViewBag.Me = userService.GetPersonBasedOnId(User.Identity.GetPersonId());
            ViewBag.Id = id;
            WallPost wp = wallService.GetWallPost(WallPostId);
            return View("_EditWallPostPartial", wp);
        }

        // USed for editting a wall post
        [HttpPost("User/{id}/EditPost/{WallPostId}", Name = "UserSubmitEditWallPost")]
        public IActionResult EditWallPost(int id, int WallPostId, WallPost wp)
        {
            ViewBag.Me = userService.GetPersonBasedOnId(User.Identity.GetPersonId());
            if (ModelState.IsValid)
            {
                wp.WallPostId = WallPostId;
                wallService.EditWallPost(wp);
                return RedirectToAction(nameof(Index), new { id = id });
            }
            return View("_EditWallPostPartial", wp);
        }

        // USed for editting a reply post
        [HttpGet("User/{id}/EditReplyPost/{ReplyPostId}", Name = "UserEditReplyPost")]
        public IActionResult EditReplyPost(int id, int ReplyPostId)
        {
            ViewBag.Me = userService.GetPersonBasedOnId(User.Identity.GetPersonId());
            ViewBag.Id = id;
            ReplyPost rp = wallService.GetReplyPost(ReplyPostId);
            return View("_EditReplyPostPartial", rp);
        }

        // USed for editting a reply post
        [HttpPost("User/{id}/EditReplyPost/{ReplyPostId}", Name = "UserSubmitEditReplyPost")]
        public IActionResult EditReplyPost(int id, int ReplyPostId, ReplyPost rp)
        {
            ViewBag.Me = userService.GetPersonBasedOnId(User.Identity.GetPersonId());
            if (ModelState.IsValid)
            {
                rp.ReplyPostId = ReplyPostId;
                wallService.EditReplyPost(rp);
                return RedirectToAction(nameof(Index), new { id = id });
            }

            return View("_EditReplyPostPartial", rp);
        }

        // Doesn't work if you put a HttpDelete tag on this. Otherwise this works fine
        public IActionResult DeleteWallPost(int id, int WallPostId)
        {
            wallService.DeleteWallPost(WallPostId);
            return RedirectToAction(nameof(Index), new { id = id });
        }

        // Doesn't work if you put a HttpDelete tag on this. Otherwise this works fine
        public IActionResult DeleteReplyPost(int id, int ReplyPostId)
        {
            wallService.DeleteReplyPost(ReplyPostId);
            return RedirectToAction(nameof(Index), new { id = id });
        }


        [HttpGet("/User/{id}/Friends", Name = "UserFriends")]
        public IActionResult Friends(int id)
        {
            ViewBag.Me = userService.GetPersonBasedOnId(User.Identity.GetPersonId());
            ViewBag.Controller = "User";
            List<Person> friends = userService.GetFriends(id);
            ViewBag.ViewerId = User.Identity.GetPersonId();
            ViewBag.Friends = friends;
            return View("_FriendsPartial");
        }

        [HttpGet("/User/{id}/MyGroups", Name = "UserGroups")]
        public IActionResult Groups(int id)
        {
            ViewBag.Me = userService.GetPersonBasedOnId(User.Identity.GetPersonId());
            ViewBag.Controller = "User";
            List<Group> groups = groupService.GetGroupsOfUser(id);
            ViewBag.Groups = groups;
            return View("_GroupsPartial");
        }

        [HttpGet("/User/{id}/MyBlogs", Name = "UserBlogs")]
        public IActionResult Blogs(int id)
        {
            ViewBag.Me = userService.GetPersonBasedOnId(User.Identity.GetPersonId());
            List<Blog> blogs = blogService.GetBlogsOfUser(id);
            ViewBag.Blogs = blogs;
            return View("_BlogsPartial");
        }

        [HttpGet("/User/{id}/MyStores", Name = "UserStores")]
        public IActionResult Stores(int id)
        {
            ViewBag.Me = userService.GetPersonBasedOnId(User.Identity.GetPersonId());
            List<Store> stores = storeService.GetStoresOfUser(id);
            ViewBag.Stores = stores;
            return View("_StoresPartial");
        }

        [HttpGet("/User/{id}/MyReviews", Name = "UserReviews")]
        public IActionResult Reviews(int id)
        {
            ViewBag.Me = userService.GetPersonBasedOnId(User.Identity.GetPersonId());
            List<StoreItem> itemsReviewed = storeService.GetReviewsOfUser(id);
            ViewBag.ItemsReviewed = itemsReviewed;
            return View("_ReviewsPartial");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
