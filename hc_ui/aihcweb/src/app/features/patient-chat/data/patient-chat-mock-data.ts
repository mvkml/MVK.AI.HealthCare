import { ChatMessage } from '../../../shared/models/chat-message.model';
import { ChatHistoryItem, ChatRailPersona } from '../../../shared/components/chat-rail/chat-rail';

// Mock-only — no HealthcareQueryTool wired in yet (US007), and there is no Patient backend at
// all (unlike Doctor's Module 1). Scoped strictly to the asking patient's own record — no
// cross-patient queries, unlike the Doctor persona's "recent visiting patients" aggregate view.
export const MOCK_PATIENT_CHAT_HISTORY: ChatMessage[] = [
  {
    id: 'p1',
    role: 'user',
    time: '10:05',
    text: 'When is my next appointment?'
  },
  {
    id: 'p2',
    role: 'assistant',
    time: '10:05',
    text: 'Your next appointment is scheduled for Jul 22, 2026 with Dr. Rosalind Okafor (Cardiology).',
    sourceTag: 'Source: your appointments only'
  },
  {
    id: 'p3',
    role: 'user',
    time: '10:06',
    text: 'What medications am I currently on?'
  },
  {
    id: 'p4',
    role: 'assistant',
    time: '10:06',
    text: 'You are currently prescribed:',
    resultList: [
      { name: 'Atorvastatin', detail: '20mg · once daily' },
      { name: 'Lisinopril', detail: '10mg · once daily' }
    ],
    sourceTag: 'Source: your medication list only'
  }
];

export const MOCK_PATIENT_HISTORY_ITEMS: ChatHistoryItem[] = [
  { label: 'Next appointment', group: 'Today', active: true },
  { label: 'Current medications', group: 'Today', active: false },
  { label: 'Last visit summary', group: 'Earlier', active: false }
];

export const MOCK_PATIENT_PERSONA: ChatRailPersona = {
  name: 'Marcus Webb',
  role: 'Patient'
};
