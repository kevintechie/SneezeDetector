# SneezeDetector
Detect loud noises using comparator and mic with Netduino 2 plus

This project was designed to send a tweet whenever it detected a loud noise. The noise being detected was the insanely loud sneeze of a co-worker.

The program uses a circuit with a electret microphone and a comparator to trigger a state change on a pin on the Netduino. The tweeting feature uses a Twitter proxy because the Netduino 2 isn't capable of implementing OAuth. Current time is set from an NTP server and added to each tweet to prevent being blocked by Twitter if two sneezes with the same message are sent.
