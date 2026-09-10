# Nudge — Product Scope & Features

Raw material from a grilling session on the core product's user stories and features, organized
for later use writing specs and tracker tickets (one feature area ≈ one future spec/ticket). See
[ARCHITECTURE.md](ARCHITECTURE.md) and [ROADMAP.md](ROADMAP.md) for the technical/phasing
counterparts — this file is the product-behavior source of truth they should stay consistent with.

## Status legend

- **v1** — in scope for the first shippable version.
- **Later phase** — explicitly in scope for the project, deliberately not v1.
- **Deferred** — no phase commitment; revisit if/when it becomes relevant.

---

## 1. Accounts & Identity — v1

Nudge is multi-user from the start, identified by Telegram user ID (no password store).

**Decisions**
- First `/start` message implicitly creates the `User` account — no separate registration/consent
  step.
- Decks are private to their owner; no cross-user visibility of any kind in v1.

**User stories**
1. As a new Telegram user, I want messaging the bot for the first time to just work, so that I don't hit a signup wall before I can use the tool.
2. As a user, I want my decks and cards to be visible only to me, so that personal notes stay private.

---

## 2. Decks — v1

**Decisions**
- Full CRUD: create, view/list, edit (title/description), archive, delete.
- Archiving a Deck cascades: its Cards are hidden from review too. No independent per-card archive
  state.
- Deleting a Deck cascades: soft-deletes its Cards, consistent with archive behavior (never a hard
  delete, per architecture rules).
- "Move a card to a different deck" (reorganization) is explicitly deferred — not v1.

**User stories**
3. As a user, I want to create a deck with a title and optional description, so that I can group related cards (e.g. "History", "English words").
4. As a user, I want to list my decks, so that I can see what topics I'm tracking.
5. As a user, I want to view a single deck's cards, so that I can review or manage its content.
6. As a user, I want to edit a deck's title/description, so that I can correct or refine it later.
7. As a user, I want to archive a deck, so that I can retire a topic without losing its data or having it clutter active review.
8. As a user, when I archive a deck, I want its cards automatically excluded from review, so that I don't have to archive each card individually.
9. As a user, I want to delete a deck, so that I can remove a topic I no longer want at all.
10. As a user, when I delete a deck, I want its cards removed too, so that I don't end up with orphaned cards.

---

## 3. Cards — v1

**Decisions**
- A card is either a **plain note** or a **front/back Q&A pair** — not a fixed type. Whether it
  has an answer is just a populated-or-not field, editable at any time (add/remove an answer on
  an existing card).
- Full CRUD: create, view/list, edit, delete.
- No independent archive state for cards (see Decks §2 — archiving is deck-level, cascading).

**User stories**
11. As a user, I want to create a card with just a note, so that I can capture something I don't want to forget without forcing it into a question format.
12. As a user, I want to create a card with a front and an answer, so that I can quiz myself on recall later.
13. As a user, I want to add an answer to an existing note-only card, so that I can turn a passive note into a quizzable one without recreating it.
14. As a user, I want to remove the answer from a card, so that I can turn a Q&A card back into a plain note if quizzing on it stops making sense.
15. As a user, I want to edit a card's content, so that I can fix mistakes or update it.
16. As a user, I want to delete a card, so that I can remove something no longer relevant.
17. As a user, I want to list/view the cards in a deck, so that I can see what I've captured there.

---

## 4. Review Engine — v1

Two independent axes: **due vs. practice**, and **per-deck vs. cross-deck**. All four combinations
are available on demand; the daily digest (§6) is just one trigger for the due/cross-deck
combination specifically.

**Decisions**
- Scheduling algorithm: SM-2 (Anki-style).
- Grading is **binary**: remembered / forgot (not 4-point Anki-style) — chosen to map cleanly onto
  chat-bot inline buttons.
- **Due sessions** pull cards whose SM-2 schedule says they're due; grading updates the SM-2 state
  (interval, ease, next-due-date).
- **Practice sessions** pull cards regardless of due date (random) and are **schedule-neutral** —
  grading in a practice session never touches SM-2 state. Pure extra practice.
- Both due and practice sessions can be scoped to **one deck** or **cross-deck** (all decks), and
  both are triggerable **on demand**, not just via the digest.
- Q&A card review flow: show front → user requests reveal → answer shown → *then* graded
  (reveal-before-grade, so grading is an honest self-check against what was just seen, not a
  blind guess).
- Note-only card review flow: full note text shown upfront; "remembered" means "still
  relevant/worth keeping."
- Sessions are **capped** at a fixed batch size (default 20 in v1) to avoid an unbounded wall of
  cards on a heavy day. The cap becomes user-configurable in a later phase (see §9 Settings); v1
  ships with the fixed default only.

**User stories**
18. As a user, I want to start a due-review session for one deck, so that I can focus my review time on a specific topic.
19. As a user, I want to start a due-review session across all my decks, so that I can review everything that's due in one sitting.
20. As a user, I want to start a random practice session for one deck, so that I can reinforce a topic without waiting for cards to become due.
21. As a user, I want to start a random practice session across all decks, so that I can get some general practice in anytime.
22. As a user, I want practicing to never mess with my real review schedule, so that idle practice doesn't distort when cards are actually due again.
23. As a user reviewing a Q&A card, I want to see the front, reveal the answer myself, and then grade whether I remembered it, so that grading reflects an honest self-assessment.
24. As a user reviewing a note-only card, I want to see the full note and just confirm it's still worth keeping, so that resurfacing notes doesn't feel like a forced quiz.
25. As a user, I want a review session to stop after a reasonable number of cards, so that a heavy due-day doesn't turn into an overwhelming, unbounded chat session.
26. As a user with more due cards than the session cap, I want to be offered another session to continue, so that I can still get through everything due, just in manageable batches.

---

## 5. Telegram Bot Interaction Model — v1

**Decisions**
- Slash commands drive actions (`/newdeck`, `/newcard`, `/review [deck]`, `/practice [deck]`,
  edit/delete/archive variants, etc.).
- Inline keyboards drive selection steps (which deck, reveal-answer, remembered/forgot grading) —
  avoids free-text parsing ambiguity for the most frequent interactions.
- The bot is the primary/only v1 client. REST (`Nudge.Api`) stays admin/testing-only in v1, per
  architecture rules — no bot-equivalent feature needs a REST endpoint yet.

**User stories**
27. As a user, I want to manage decks and cards via typed commands, so that actions are explicit and discoverable.
28. As a user, I want to pick a deck or grade a card via tappable buttons, so that I don't have to type free-text answers for simple choices.

---

## 6. Quick Capture & Inbox — v1

**Decisions**
- Sending the bot a plain message with no active command creates a **note-only card** immediately
  — no command required to capture a fleeting thought.
- That card goes into a special deck called **Inbox**, auto-created and **fully protected**: it
  cannot be deleted, archived, or renamed. Only its cards can be added/edited/removed.
- Any card created without an explicitly named deck (quick-capture, or a deck-less `/newcard`)
  routes to Inbox.

**User stories**
29. As a user, I want to just send the bot a message to capture a thought, so that capturing something has zero friction.
30. As a user, I want quick-captured notes to land somewhere findable by default, so that I don't lose them even if I didn't think about organization at capture time.
31. As a user, I want the Inbox deck to always exist and never be accidentally deleted or archived, so that quick capture never silently breaks.

---

## 7. Daily Digest — v1

**Decisions**
- A proactive push at a **fixed time** (same for all users in v1 — no per-user timezone/schedule
  config yet; see §9 Settings).
- Content: due cards, **cross-deck** (matches the default due-session scope).
- If there are zero due cards that day, the digest instead offers a **practice session** (since
  practice is always available and schedule-neutral) rather than sending nothing or a bare "no
  cards due" message.

**User stories**
32. As a user, I want to be proactively nudged once a day when I have cards due, so that I don't have to remember to check myself.
33. As a user with nothing due on a given day, I want to be offered a quick practice session instead of silence, so that the bot still gives me a reason to engage that day.

---

## 8. Search — Later phase (in scope, not v1)

**Decisions**
- Keyword search across all decks, including Inbox.
- Triggered by an **explicit command** (e.g. `/search <keyword>`) — not automatic/implicit.
- Explicitly scoped for a later phase because Inbox-driven quick capture (§6) is expected to
  accumulate unsorted content that becomes worth searching only once there's enough of it.

**User stories**
34. As a user, I want to search my cards by keyword across all decks, so that I can find something I captured but don't remember where I filed it.
35. As a user, I want search to include Inbox, so that quick-captured, unsorted notes are still findable.

---

## 9. User Settings — Later phase (in scope, not v1)

**Decisions**
- Grouped feature covering at least: review session cap (§4) and daily digest time (§7).
- v1 ships with fixed defaults for both (session cap = 20, one fixed digest time for everyone);
  no settings storage or command exists in v1.
- Deliberately built as one `Settings`-shaped feature later rather than one-off configurability
  added piecemeal per knob.

**User stories**
36. As a user, I want to configure how many cards appear in a review session, so that I can tune sessions to my own attention span/available time.
37. As a user, I want to configure what time I get my daily digest, so that it arrives when it's actually convenient for me (this also implies eventual per-user timezone handling).

---

## 10. Stats & Progress — Deferred (no phase commitment)

Explicitly out of scope for now, including for the later-phase items above. Revisit if/when
requested again.

**Candidate stories, not committed to any phase**
38. As a user, I want to see my current review streak, so that I have a motivating signal to keep up the habit.
39. As a user, I want to see how many cards I've reviewed today/in total, so that I can gauge my engagement over time.
40. As a user, I want to see my retention rate (remembered vs. forgot), so that I can tell whether spaced repetition is actually working for me.

---

## 11. Sharing / Social — Deferred (explicitly out for v1)

**Decisions**
- Decks are strictly private in v1 — no sharing a deck with another user, no public/browsable deck
  library, no importing someone else's deck.
- Not committed to any later phase; revisit only if requested.

---

## Cross-cutting notes for future spec-writing

- **Testing seam** (proposed, not yet confirmed by the user): primary seam = command/query handler
  boundary backed by EF Core's InMemory provider (covers cascades, session composition,
  quick-capture routing without a real Postgres/Testcontainers, consistent with the
  architecture's "unit tests only" rule); secondary, narrower seam = aggregate/domain methods
  directly (SM-2 math, business-rule edge cases). Confirm before specs lock this in.
- **Issue tracker / triage label vocabulary** is not yet configured in this repo (no
  `setup-matt-pocock-skills` output found, no `gh` CLI available in this environment) — needed
  before any of the above can be turned into published tracker tickets via `/to-spec`.
- Every "Later phase" and "Deferred" item above is a natural candidate for its own future spec
  once its phase arrives, rather than being bundled into the v1 spec(s).
