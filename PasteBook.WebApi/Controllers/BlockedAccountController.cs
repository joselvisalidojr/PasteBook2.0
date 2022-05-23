﻿using Microsoft.AspNetCore.Mvc;
using PasteBook.Data;
using PasteBook.Data.DataTransferObjects;
using PasteBook.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PasteBook.WebApi.Controllers
{
    [Route("blocked-accounts")]
    [ApiController]
    public class BlockedAccountController : ControllerBase
    {
        private readonly IUnitOfWork UnitOfWork;

        public BlockedAccountController(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        [HttpGet("get-blocked-accounts")]
        public async Task<IActionResult> GetBlockedAccounts(int id)
        {
            var blockedAccounts = await this.UnitOfWork.BlockedAccountRepository.FindByBlockerAccountId(id);

            if (blockedAccounts != null)
            {
                var blockedAccountsDTO = new List<BlockedAccountDTO>();
                foreach (var blockedAccount in blockedAccounts)
                {
                    var BlockedUserAccount = await UnitOfWork.UserAccountRepository.FindByPrimaryKey(blockedAccount.BlockerAccountId);
                    blockedAccountsDTO.Add(new BlockedAccountDTO
                    {
                        Id = blockedAccount.Id,
                        BlockerAccountId = blockedAccount.BlockerAccountId,
                        BlockedAccountId = blockedAccount.BlockedAccountId,
                        FirstName = BlockedUserAccount.FirstName,
                        LastName = BlockedUserAccount.LastName,
                        Active = BlockedUserAccount.Active,
                        ProfileImagePath = BlockedUserAccount.ProfileImagePath,
                        BlockedDate = blockedAccount.BlockedDate
                    });
                }
                return Ok(blockedAccountsDTO);
            }
            return BadRequest();
        }

        [HttpPost("block-account")]
        public async Task<IActionResult> BlockAccount(int blockerAccount, int blockedAccount)
        {
            var newBlock = new BlockedAccount
            {
                BlockerAccountId = blockerAccount,
                BlockedAccountId = blockedAccount,
                BlockedDate = DateTime.Now
            };
            await this.UnitOfWork.BlockedAccountRepository.Insert(newBlock);

            var friend = await this.UnitOfWork.FriendRepository.FindByUserAccountIds(blockerAccount, blockedAccount);
            if(friend != null)
            {
                await this.UnitOfWork.FriendRepository.Delete(friend.Id);
            }
            await this.UnitOfWork.CommitAsync();
            return Ok(newBlock);
        }

        [HttpDelete("unblock-account")]
        public async Task<IActionResult> UnblockAccount(int id)
        {
            var unblockAccount = await this.UnitOfWork.BlockedAccountRepository.Delete(id);
            await this.UnitOfWork.CommitAsync();
            return Ok(unblockAccount);
        }

        [HttpGet("check-if-blocked")]
        public async Task<IActionResult> CheckIfBlocked(int blockerId, int blockedId)
        {
            var blockedUser = await this.UnitOfWork.BlockedAccountRepository.FindByAccountIds(blockerId, blockedId);
            if (blockedUser != null)
            {
                return Ok(blockedUser);
            }
            return BadRequest();
        }
    }
}
