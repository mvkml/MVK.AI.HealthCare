import { ChatMessage } from '../models/chat-message.model';

// Mock data only — no HC.AI.MAPI call yet, pending the US007 endpoint contract.
export const MOCK_CHAT_HISTORY: ChatMessage[] = [
  {
    id: 'm1',
    role: 'user',
    time: '09:41',
    text: 'Which patients have visited in the last 7 days?'
  },
  {
    id: 'm2',
    role: 'assistant',
    time: '09:41',
    text: '6 patients were seen in the last 7 days:',
    resultList: [
      { name: 'Marcus Webb', detail: 'Internal Medicine · Jul 15' },
      { name: 'Priya Nair', detail: 'Cardiology · Jul 15' },
      { name: 'Diego Alvarez', detail: 'Internal Medicine · Jul 14' },
      { name: 'Helen Ostrowski', detail: 'Endocrinology · Jul 13' }
    ],
    sourceTag: 'Source: Encounters (last 7 days) · 6 rows · Tool Layer'
  },
  {
    id: 'm3',
    role: 'user',
    time: '09:42',
    text: 'Show me details for Priya Nair.'
  },
  {
    id: 'm4',
    role: 'assistant',
    time: '09:42',
    text: 'Priya Nair — Female, 47',
    detailFields: [
      { term: 'Last visit', value: 'Jul 15, 2026 · Cardiology' },
      { term: 'Allergies', value: 'Penicillin' },
      { term: 'Active meds', value: 'Atorvastatin, Lisinopril' }
    ],
    sourceTag: 'Source: Patients, Encounters, Medications · 3 rows joined'
  }
];

export const MOCK_HISTORY_ITEMS = [
  { label: 'Recent visiting patients', group: 'Today', active: true },
  { label: 'Patient details — Priya Nair', group: 'Today', active: false },
  { label: 'How many patients — cardiology', group: 'Earlier', active: false },
  { label: 'Allergy check — Marcus Webb', group: 'Earlier', active: false }
];

export const MOCK_PERSONA = {
  name: 'Dr. Rosalind Okafor',
  role: 'Attending — Internal Medicine'
};
