import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  OnInit
} from '@angular/core';
import { FormsModule } from '@angular/forms';

import { CreateTicketRequest, Ticket } from './models/ticket';
import { TicketService } from './services/ticket.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {

  tickets: Ticket[] = [];

  newTicket: CreateTicketRequest = {
    title: '',
    asset: '',
    description: ''
  };

  statusComments: Record<number, string> = {};
  diagnoses: Record<number, string> = {};

  errorMessage = '';
  isLoading = false;
  updatingTicketId: number | null = null;

  constructor(
    private ticketService: TicketService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadTickets();
  }

  get pendingTickets(): Ticket[] {
    return this.tickets.filter(
      ticket => ticket.status === 'Pending'
    );
  }

  get inProgressTickets(): Ticket[] {
    return this.tickets.filter(
      ticket => ticket.status === 'InProgress'
    );
  }

  get resolvedTickets(): Ticket[] {
    return this.tickets.filter(
      ticket => ticket.status === 'Resolved'
    );
  }

  loadTickets(): void {
    this.ticketService.getTickets().subscribe({
      next: tickets => {
        this.tickets = tickets;
        this.changeDetectorRef.markForCheck();
      },
      error: () => {
        this.errorMessage = 'Could not load tickets.';
        this.changeDetectorRef.markForCheck();
      }
    });
  }

  createTicket(): void {
    this.errorMessage = '';

    if (
      !this.newTicket.title.trim() ||
      !this.newTicket.asset.trim() ||
      !this.newTicket.description.trim()
    ) {
      this.errorMessage = 'All fields are required.';
      return;
    }

    this.isLoading = true;

    this.ticketService.createTicket(this.newTicket).subscribe({
      next: ticket => {
        this.tickets = [ticket, ...this.tickets];

        this.newTicket = {
          title: '',
          asset: '',
          description: ''
        };

        this.isLoading = false;
        this.changeDetectorRef.markForCheck();
      },
      error: () => {
        this.errorMessage = 'Could not create the ticket.';
        this.isLoading = false;
        this.changeDetectorRef.markForCheck();
      }
    });
  }

  startProgress(ticket: Ticket): void {
    this.errorMessage = '';
    this.updatingTicketId = ticket.ticketId;

    this.ticketService.changeStatus(ticket.ticketId, {
      newStatus: 'InProgress',
      comment: this.statusComments[ticket.ticketId] || undefined
    }).subscribe({
      next: updatedTicket => {
        this.replaceTicket(updatedTicket);
        this.statusComments[ticket.ticketId] = '';
        this.updatingTicketId = null;
        this.changeDetectorRef.markForCheck();
      },
      error: error => {
        this.errorMessage = this.getErrorMessage(error);
        this.updatingTicketId = null;
        this.changeDetectorRef.markForCheck();
      }
    });
  }

  resolveTicket(ticket: Ticket): void {
    this.errorMessage = '';

    const diagnosis = this.diagnoses[ticket.ticketId]?.trim();

    if (!diagnosis) {
      this.errorMessage = 'A diagnosis is required to resolve the ticket.';
      return;
    }

    this.updatingTicketId = ticket.ticketId;

    this.ticketService.changeStatus(ticket.ticketId, {
      newStatus: 'Resolved',
      comment: this.statusComments[ticket.ticketId] || undefined,
      diagnosis
    }).subscribe({
      next: updatedTicket => {
        this.replaceTicket(updatedTicket);

        this.statusComments[ticket.ticketId] = '';
        this.diagnoses[ticket.ticketId] = '';

        this.updatingTicketId = null;
        this.changeDetectorRef.markForCheck();
      },
      error: error => {
        this.errorMessage = this.getErrorMessage(error);
        this.updatingTicketId = null;
        this.changeDetectorRef.markForCheck();
      }
    });
  }

  private replaceTicket(updatedTicket: Ticket): void {
    this.tickets = this.tickets.map(ticket =>
      ticket.ticketId === updatedTicket.ticketId
        ? updatedTicket
        : ticket
    );
  }

  private getErrorMessage(error: any): string {
    if (typeof error?.error === 'string') {
      return error.error;
    }

    return 'Could not update the ticket.';
  }
}