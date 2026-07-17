# TASK007 - Chat feature module (Angular, mock data)

**US:** US008
**Status:** Done

## Description
Build the Angular chat UI (message list, composer, conversation-history rail) as standalone
components against mock data, translating `design/chat_mockup.html` into real
`src/app/features/chat` components. No backend call yet — blocked on the US007 endpoint contract
(tracked separately, not part of this task).

## Done
- Feature module built, wired into routing (`/chat`, lazy-loaded)
- 6 unit tests passing (`ChatPage`, `Composer`, `App`)
- `ng build` and `ng serve` both verified

## Follow-on tasks (not yet created — raise when unblocked)
- Wire `ChatPage.onSend` to the real HC.AI.MAPI Doctor endpoint once the contract is confirmed
- Loading/error states once there's a real HTTP call to load/error on
- QA to extend `US008_aihcweb_chat_ui_test_plan.md` coverage now that the feature module exists

See [worklog](../../worklogs/dev_angular/20260717_232309_chat_feature_module_mock_data.md) for
full detail.
