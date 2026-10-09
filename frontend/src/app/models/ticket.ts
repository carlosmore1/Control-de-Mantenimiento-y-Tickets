export interface Ticket {
  ticketId: number;
  title: string;
  asset: string;
  description: string;
  status: string;
  diagnosis: string | null;
  assignedOperator: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateTicketRequest {
  title: string;
  asset: string;
  description: string;
}

export interface ChangeTicketStatusRequest {
  newStatus: string;
  comment?: string;
  diagnosis?: string;
  assignedOperator?: string;
}