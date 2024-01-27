export interface NewBasicEventRequest {
	Name: string;
	Tags: string[];
	Start: Date;
	End?: Date;
}