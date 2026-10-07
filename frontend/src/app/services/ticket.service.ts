import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Ticket,
  CreateTicketRequest,
  ChangeTicketStatusRequest
} from '../models/ticket';

@Injectable({
  providedIn: 'root'
})
export class TicketService {

  private readonly apiUrl = 'http://localhost:5054/api/tickets';

  constructor(private http: HttpClient) {}

  getTickets(): Observable<Ticket[]> {
    return this.http.get<Ticket[]>(this.apiUrl);
  }

  createTicket(request: CreateTicketRequest): Observable<Ticket> {
    return this.http.post<Ticket>(this.apiUrl, request);
  }

  changeStatus(
    ticketId: number,
    request: ChangeTicketStatusRequest
  ): Observable<Ticket> {
    return this.http.put<Ticket>(
      `${this.apiUrl}/${ticketId}/status`,
      request
    );
  }
}