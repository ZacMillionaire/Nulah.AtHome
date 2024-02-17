import type { EventTag } from './EventTag';

export interface BasicEvent {
	Description: string;
	Id: number;
	Tags: EventTag[] | null;
	Start: Date;
	End: Date | null;
	Version: number;
}