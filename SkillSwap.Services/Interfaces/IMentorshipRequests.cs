﻿using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Interfaces;

public interface IMentorshipRequests
{
    Task<MentorshipRequests> GetMentorshipRequestById(Guid id);
    Task<MentorshipRequests> GetMentorshipRequestsbyLearnerId(Guid learnerId);
    Task<MentorshipRequests> GetMentorshipRequestsbyMentorId(Guid mentorId);
    Task<MentorshipRequests> CreateMentorshipRequest(MentorshipRequests mentorshipRequest);
    Task<MentorshipRequests> UpdateMentorshipRequest(Guid id, MentorshipRequests updateMentorshipRequest);
    Task DeleteMentorshipRequest(Guid id);
}
