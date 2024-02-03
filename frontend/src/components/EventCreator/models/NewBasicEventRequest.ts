export interface NewBasicEventRequest {
	Description: string;
	Tags: string[] | null;
	Start: Date;
	End: Date | null;
}