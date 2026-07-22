export interface ChatResultRow {
  name: string;
  detail: string;
}

export interface ChatDetailField {
  term: string;
  value: string;
}

export interface ChatMessage {
  id: string;
  role: 'user' | 'assistant';
  time: string;
  text?: string;
  resultList?: ChatResultRow[];
  detailFields?: ChatDetailField[];
  sourceTag?: string;
}
