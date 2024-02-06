<script lang="ts">
	import EventList from '../../components/EventCreator/EventList.svelte';
	import EventCreator from '../../components/EventCreator/EventCreator.svelte';
	import { EventStore } from '../../components/EventCreator/EventStore';
</script>
<h1>Hello events</h1>
<a href="/">home</a>

<div style="width: 400px">
	<EventCreator formSubmitAction="{EventStore.CreateEvent}" />
	<EventList />
</div>
