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

  errorMessage = '';
  isLoading = false;

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
        // Add the new ticket immediately without making another GET request.
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
}